using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace SR.UI
{
	public class SettingsMenuUI : MenuBase
	{
		#region Variables

		[SerializeField] private TMP_Dropdown languageDD;
		[SerializeField] private Toggle soundToggle;
		[SerializeField] private Button backButton;

		private List<LocaleIdentifier> langCodes = new List<LocaleIdentifier>();

		#endregion

		#region UnityMessages

		private void Awake()
		{
			InitLocales();

			languageDD.onValueChanged.AddListener(x =>
			{
				LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(langCodes[x]);
			});

			backButton.onClick.AddListener(() =>
			{
				BackToPrevious();
				Close();
			});

			soundToggle.onValueChanged.AddListener(x =>
			{

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

