using Cinemachine;
using SR.Customization;
using SR.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;
using Zenject;

namespace SR.Core
{
	public class Garage : MonoBehaviour
	{
		#region Variables

		[Header("Components")]
		[SerializeField] private GameObject garageToDestroyOnHide;
		[SerializeField] private GarageCarCustomizer garageCarCustomizer;
		[Inject] private GameplayBase gameplayBase;
		[Inject] private GameInstance gameInstance;

		#endregion

		#region UnityMessages

		private void Awake()
		{
			gameInstance.onDetailChanged += GameInstance_onDetailChanged;
		}

		private void Start()
		{
#if UNITY_ANDROID
				Init();
#elif UNITY_WEBGL
#if UNITY_EDITOR
			if (YandexGame.auth)
			{
#endif
				Init();
#if UNITY_EDITOR
			}
			else
			{
				StartCoroutine(DelayedInit());
			}
#endif
		}
		private IEnumerator DelayedInit()
		{
			float time = 3f;
			while (time > 0)
			{
				time -= Time.deltaTime;
				yield return null;

				if (YandexGame.auth)
				{
					gameInstance.GetShopLibrary().Initialize();
					Init();
					gameInstance.CallUpdateCallbacks();
					yield break;
				}
			}
		}
#endif

		private void OnDestroy()
		{
			gameInstance.onDetailChanged -= GameInstance_onDetailChanged;
		}

		#endregion

		#region Functions

		private void Init()
		{
			var car = gameInstance.GetCarConfig();
			garageCarCustomizer.SetDetail(gameInstance.GetShopLibrary().GetBumper(car.bumper));
			garageCarCustomizer.SetDetail(gameInstance.GetShopLibrary().GetBackdoor(car.backdoor));
			garageCarCustomizer.SetDetail(gameInstance.GetShopLibrary().GetWheels(car.wheels));
			garageCarCustomizer.SetDetail(gameInstance.GetShopLibrary().GetWeapon(car.weapon));
			garageCarCustomizer.SetDetail(gameInstance.GetShopLibrary().GetStickman(car.stickman));
		}

		#endregion

		#region Callbacks

		private void GameInstance_onDetailChanged(object sender, GameInstance.DetailEventArgs e)
		{
			garageCarCustomizer.SetDetail(e.detail);
		}

		#endregion

	}
}
