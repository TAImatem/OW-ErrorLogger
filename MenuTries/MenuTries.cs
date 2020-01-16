using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

namespace MenuTries
{
	public class MenuTries : ModBehaviour
	{
		public static IModConsole console;
		public static GameObject extramenu, canvas, toggle, rebinder;
		public static Transform content;
		public static Button menubutton;
		public static Menu cusMenu;
		public static Selectable first=null;

		private void Start()
		{
			Application.logMessageReceived += OnLogMessageReceived;
			console = ModHelper.Console;

			menubutton = ModHelper.Menus.MainMenu.AddButton("Custom Menu", 4);
			menubutton.onClick.AddListener(OnEvent);

			ModHelper.Console.WriteLine("copying menu");
			canvas = GameObject.Find("KeyboardRebindingCanvas");
			extramenu = canvas.transform.GetChild(0).gameObject;
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
			var rtext = extramenu.transform.Find("FooterPanel").GetComponentsInChildren<Text>();
			rtext[0].text = "Cancel and Exit";
			rtext[1].text = "Default settings";
			rtext[2].text = "Save and Exit";
			rtext[3].text = "Discard changes";

			GameObject newins;
			Selectable prev=null, cur;
			for (int i = 1; i <= 5; i++)
			{
				newins = GameObject.Instantiate(toggle, content);
				cur = newins.GetComponent<Button>();
				newins.GetComponent<TwoButtonToggleElement>().Initialize(false);	
				Navigation nav = cur.navigation, nav2;
				nav.mode = Navigation.Mode.Explicit;
				if (i == 1)
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
				if (i == 5)
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
				//var temp = newins.transform.GetChild(1).GetChild(1);
				//temp.GetChild(0).GetComponentInChildren<Text>().text = "Enabled";
				//temp.GetChild(1).GetComponentInChildren<Text>().text = "Disabled"; //probably should rather somehow keep the LocalizedText being destroyed from those two
			}
			extramenu.transform.GetChild(7).gameObject.SetActive(false);
			ModHelper.Console.WriteLine("MenuTry done!");
		}

		private void OnLogMessageReceived(string message, string stackTrace, LogType type)
		{
				if (stackTrace != null)
					ModHelper.Logger.Log($"\t{type}: {message}; {stackTrace}");
				else
					ModHelper.Logger.Log($"\t{type}: {message}");
		}
		private void OnEvent()
		{
			ModHelper.Console.WriteLine("trying to open menu");
			cusMenu.EnableMenu(true);
			Locator.GetMenuInputModule().SelectOnNextUpdate(first);
			ModHelper.Console.WriteLine("trying to open menu (end)");
		}			
	}
}
