using SR.Core;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Zenject;

namespace SR.UI
{
	public class SettingsMenuUI : MenuBase
	{
		#region Variables

		public static event EventHandler onLanguageChanged;

		[SerializeField] private TMP_Dropdown languageDD;
		[SerializeField] private Button soundToggle;
		[SerializeField] private Image soundImage;
		[SerializeField] private Button backButton;

		[SerializeField] private Sprite soundsOn;
		[SerializeField] private Sprite soundsOff;

		[Inject] GameInstance gameInstance;
		[Inject] SoundSystem soundSystem;

		private List<LocaleIdentifier> langCodes = new List<LocaleIdentifier>();

		#endregion

		#region UnityMessages

		private void Awake()
		{
			InitLocales();

			soundImage.sprite = soundSystem.IsSoundEnabled() ? soundsOn : soundsOff;

			languageDD.onValueChanged.AddListener(x =>
			{
				LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(langCodes[x]);
				onLanguageChanged?.Invoke(this, EventArgs.Empty);
			});

			backButton.onClick.AddListener(() =>
			{
				soundSystem.PlayButton2();
				gameInstance.ConfirmSave();
				BackToPrevious();
				Close();
			});

			soundToggle.onClick.AddListener(() =>
			{
				soundSystem.PlayButton1();
				if (soundSystem.IsSoundEnabled())
				{
					soundSystem.DisableSound();
					soundImage.sprite = soundsOff;
				}
				else
				{
					soundSystem.EnableSound();
					soundImage.sprite = soundsOn;
				}
				gameInstance.SetSounds(soundSystem.IsSoundEnabled());
				gameInstance.SaveGameSettings();
			});
		}

		#endregion

		#region Functions

		private void InitLocales()
		{
			var initOp = LocalizationSettings.SelectedLocaleAsync;

			if (initOp.IsDone)
			{
				UpdateLocalesDD();
			}
			else
			{
				initOp.Completed += x =>
				{
					UpdateLocalesDD();
				};
			}
		}

		private void UpdateLocalesDD()
		{
			langCodes.Clear();
			languageDD.ClearOptions();

			List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();

			foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
			{
				TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
				optionData.text = locale.LocaleName;
				optionData.image = null;
				list.Add(optionData);
				langCodes.Add(locale.Identifier);
			}

			languageDD.AddOptions(list);

			Locale currentLocale = LocalizationSettings.SelectedLocale;

			for (int i = 0; i < langCodes.Count; i++)
			{
				if (currentLocale.Identifier == langCodes[i])
				{
					languageDD.SetValueWithoutNotify(i);
					break;
				}
			}

			languageDD.RefreshShownValue();
		}

		#endregion
	}
}

