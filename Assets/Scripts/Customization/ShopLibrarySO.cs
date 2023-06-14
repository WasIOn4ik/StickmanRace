using SR.Core;
using SR.Customization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SR/ShopLibrary")]
public class ShopLibrarySO : ScriptableObject
{
	public List<BumperSO> bumpers;
	public List<WheelsSO> wheels;
	public List<BackDoorSO> backdoors;
	public List<WeaponSO> weapons;
	public List<StickmanSO> stickmans;

	public BumperSO startBumper;
	public WheelsSO startWheels;
	public BackDoorSO startBackDoor;
	public WeaponSO startWeapon;
	public StickmanSO startStickman;

	public CarConfig GetStandartCar()
	{
		return new CarConfig()
		{
			backdoor = startBackDoor.identifier,
			bumper = startBumper.identifier,
			wheels = startWheels.identifier,
			stickman = startStickman.identifier,
			weapon = startWeapon.identifier
		};
	}

	public BumperSO GetBumper(string id)
	{
		return bumpers.Find(x => { return x.identifier == id; });
	}

	public WheelsSO GetWheels(string id)
	{
		return wheels.Find(x => { return x.identifier == id; });
	}

	public BackDoorSO GetBackdoor(string id)
	{
		return backdoors.Find(x => { return x.identifier == id; });
	}

	public WeaponSO GetWeapon(string id)
	{
		return weapons.Find(x => { return x.identifier == id; });
	}

	public StickmanSO GetStickman(string id)
	{
		return stickmans.Find(x => { return x.identifier == id; });
	}

	public List<CarDetailSO> GetCategoryList(CarDetailType category)
	{
		List<CarDetailSO> list = new List<CarDetailSO>();
		switch (category)
		{
			case CarDetailType.Wheels:
				foreach(var el in wheels)
				{
					list.Add(el);
				}
				break;
			case CarDetailType.Bumper:
				foreach (var el in bumpers)
				{
					list.Add(el);
				}
				break;
			case CarDetailType.BackDoor:
				foreach (var el in backdoors)
				{
					list.Add(el);
				}
				break;
			case CarDetailType.Weapon:
				foreach (var el in weapons)
				{
					list.Add(el);
				}
				break;
			case CarDetailType.Stickman:
				foreach (var el in stickmans)
				{
					list.Add(el);
				}
				break;
		}

		return list;
	}
}
