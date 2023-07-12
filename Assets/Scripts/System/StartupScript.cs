using SR.UI;
using UnityEngine;
using UnityEngine.Localization.Settings;
#if UNITY_ANDROID
using CAS.AdObject;
using CAS;
using Zenject;
using SR.Core;
using GooglePlayGames;
#endif

namespace SR.Extras
{
	public class StartupScript : MonoBehaviour
	{
#if UNITY_ANDROID
		[SerializeField] private ManagerAdObject androidADManager;
		[SerializeField] private BannerAdObject bannerAdManager;
		[SerializeField] private InterstitialAdObject interstitialAdManager;
		[SerializeField] private RewardedAdObject rewardedAdManager;

		[Inject] GameInstance gameInstance;
#endif
#region UnityMessages

		private void Start()
		{
#if UNITY_ANDROID
			androidADManager.OnInitialized.AddListener(OnAndroidADInitialized);
			androidADManager.Initialize();
			gameInstance.banner = bannerAdManager;
			if (gameInstance.bNoAdsBought)
				gameInstance.SetBannerActivity(false);
			gameInstance.interstitial = interstitialAdManager;
			gameInstance.rewarded = rewardedAdManager;
			DontDestroyOnLoad(bannerAdManager);
			DontDestroyOnLoad(interstitialAdManager);
			DontDestroyOnLoad(rewardedAdManager);
#endif
			//TODO: Loading screen
			var localeOperation = LocalizationSettings.SelectedLocaleAsync;

			if (localeOperation.IsDone)
			{
				ShowMainMenu();
			}
			else
			{
				localeOperation.Completed += x =>
				{
					ShowMainMenu();
				};
			}
		}

		#endregion

		#region Functions

#if UNITY_ANDROID
		private void OnAndroidADInitialized()
		{

		}
#endif

		private void ShowMainMenu()
		{
			MenuBase.OpenMenu(MenuType.MainMenu, true);
		}

		#endregion
	}
}
