using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using System.Security.AccessControl;
using System.Collections.Generic;

namespace MenuTries
{
	public class MenuOpener : MonoBehaviour
	{
		private Menu menu;
		private Button button = null;
		public void Initialize (Menu toOpen, Button toListen)
		{
			if (button!= null)
				button.onClick.RemoveListener(OnEvent);
			menu = toOpen;
			button = toListen;
			button.onClick.AddListener(OnEvent);
		}
		private void OnEvent()
		{
			menu.EnableMenu(true);
		}
	}

	public class MenuTries : ModBehaviour
	{
		public static IModConsole console;
		public static GameObject extramenu, canvas, toggle, rebinder;
		public static Transform content;
		public static Button menubutton;
		public static Menu cusMenu, somemenu;
		public static Selectable first=null;
		private static Dictionary<GameObject, ModBehaviour> dict = new Dictionary<GameObject, ModBehaviour>();

		private void Start()
		{
			Application.logMessageReceived += OnLogMessageReceived;
			console = ModHelper.Console;

			menubutton = ModHelper.Menus.MainMenu.AddButton("Custom Menu", 4); //button setup, as well as parenting should be moved to a listener, so it will re-setup after level change
			menubutton.onClick.AddListener(OpenMenu);

			ModHelper.Console.WriteLine("copying menu");
			canvas = GameObject.Find("KeyboardRebindingCanvas");
			extramenu = canvas.transform.GetChild(0).gameObject;
			somemenu = extramenu.GetComponent<Menu>();
			extramenu = GameObject.Instantiate(extramenu,canvas.transform);
			cusMenu = extramenu.GetComponent<Menu>();

			ModHelper.Console.WriteLine("setting up controls");
			content = extramenu.transform.Find("DetailsPanel").GetChild(2).GetChild(0).GetChild(0).GetChild(0);
			GameObject.Destroy(extramenu.transform.Find("DetailsPanel").GetChild(1).gameObject);
			rebinder = content.GetChild(0).gameObject;
			toggle = GameObject.Find("OptionsCanvas").transform.GetChild(0).GetChild(4).GetChild(3).GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject;
			toggle = Instantiate(toggle, content);
			toggle.SetActive(false);

			ModHelper.Console.WriteLine("setting up text");
			var ltext = extramenu.GetComponentsInChildren<LocalizedText>();
			for (int i = 0; i < ltext.Length; i++)
				GameObject.Destroy(ltext[i]);
			GameObject.Destroy(toggle.GetComponentInChildren<LocalizedText>());

			content.GetChild(0).gameObject.SetActive(false);

			extramenu.transform.Find("HeaderPanel").GetComponentInChildren<Text>().text = "Custom Menu";

			var footer = extramenu.transform.Find("FooterPanel");
			var rtext = footer.GetComponentsInChildren<Text>();
			rtext[0].text = "Cancel and Exit";
			rtext[1].text = "Default settings";
			rtext[2].text = "Save and Exit";
			rtext[3].text = "Discard changes";

			footer.GetChild(1).GetComponent<Button>().onClick.AddListener(SetDefault);
			footer.GetChild(2).GetComponent<Button>().onClick.AddListener(SaveSettings);
			footer.GetChild(3).GetComponent<Button>().onClick.AddListener(Discard);
			extramenu.transform.GetChild(7).gameObject.SetActive(false);
			ModHelper.Console.WriteLine("MenuTry done!");
		}

		private void OpenMenu()
		{
			if (dict.Count == 0)
				InitializeMenu();
			cusMenu.EnableMenu(true);
			Locator.GetMenuInputModule().SelectOnNextUpdate(first);
		}
		private void InitializeMenu()
		{
			//InitializeMenu(GetModsList());
		}

		public void InitializeMenu(ModBehaviour[] mods)//maybe not ModBehaviours, since those will probably be null if disabled
		{
			int size = mods.Length;
			GameObject newins;
			Selectable prev = null, cur;
			Navigation nav, nav2;
			for (int i = 0; i < size; i++)
			{
				newins = GameObject.Instantiate(toggle, content);
				cur = newins.GetComponent<Button>();
				//create mod's own menu here
				(newins.AddComponent<MenuOpener>() as MenuOpener).Initialize(somemenu, cur as Button); //replace somemenu with actuall mod's menu

				dict.Add(newins, mods[i]);

				newins.GetComponent<TwoButtonToggleElement>().Initialize(mods[i].isActiveAndEnabled);
				nav = cur.navigation;
				nav.mode = Navigation.Mode.Explicit;
				if (i == 0)
				{
					cusMenu.SetSelectOnActivate(cur);
					first = cur;
				}
				else
				{
					nav2 = prev.navigation;
					nav2.selectOnDown = cur;
					nav.selectOnUp = prev;
					prev.navigation = nav2;
				}
				if (i == size)
				{
					nav2 = first.navigation;
					nav.selectOnDown = first;
					nav2.selectOnUp = cur;
					first.navigation = nav2;
				}
				newins.GetComponentInChildren<Text>().text = $"mod #{i}";
				newins.SetActive(true);
				cur.navigation = nav;
				prev = cur;
			}
		}

		private void Discard()
		{
			foreach (var key in dict.Keys)
			{
				key.GetComponent<TwoButtonToggleElement>().Initialize(dict[key].isActiveAndEnabled);
			}
		}

		private void SetDefault()
		{
			foreach (var key in dict.Keys)
			{
				//key.GetComponent<TwoButtonToggleElement>().Initialize(dict[key].defaultConfig.GetValue("enabled"));
			}
			SaveSettings();
		}

		private void SaveSettings()
		{
			foreach (var key in dict.Keys)
			{
				bool newval = key.GetComponent<TwoButtonToggleElement>().GetValue();
				bool oldval = dict[key].isActiveAndEnabled;
				if (oldval^newval)
				{
					if (newval)
					{
						//dict[key].Initialize or whatever
					}
					else
					{
						//dict[key].UnPatchAll();
						GameObject.Destroy(dict[key]); // or maybe disable but make awake and start rerun itself
					}
				}
			}
		}


		private void OnLogMessageReceived(string message, string stackTrace, LogType type)
		{
				if (stackTrace != null)
					ModHelper.Logger.Log($"\t{type}: {message}; {stackTrace}");
				else
					ModHelper.Logger.Log($"\t{type}: {message}");
		}		
	}
}
