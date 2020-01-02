using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace MenuTries
{
	class MenuPatch
	{
		public static void Menu_Prefix(TitleScreenManager __instance)
		{
			GameObject mainmenu = GameObject.Find("MainMenuLayoutGroup");
			if (mainmenu)
			{
				MenuTries.console.WriteLine("Menu " + mainmenu.name);
				MenuTries.setbut = mainmenu.transform.Find("Button-Options").gameObject;
				if (MenuTries.setbut)
				{
					MenuTries.console.WriteLine("Button acquired");
					MenuTries.setbut.GetComponentInChildren<Text>().text = "newsettings";
					if (mainmenu.name == "MainMenuLayoutGroup")
					{

						MenuTries.extrabut = GameObject.Instantiate(MenuTries.setbut);
						MenuTries.extrabut.transform.SetParent(mainmenu.transform, false);
						MenuTries.extrabut.transform.position = MenuTries.setbut.transform.position;
						MenuTries.extrabut.GetComponentInChildren<Text>().text = "newtext";
						MenuTries.extrabut.SetActive(true);
						mainmenu.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputHorizontal();
						mainmenu.GetComponent<VerticalLayoutGroup>().SetLayoutHorizontal();
						MenuTries.console.WriteLine("Button added!");
					}
				}
			}
		}
	}
	public class MenuTries : ModBehaviour
	{
		public static IModConsole console;
		public static GameObject extrabut, mainmenu, setbut;

		private void Start()
		{
			console = ModHelper.Console;
			ModHelper.HarmonyHelper.AddPrefix<TitleScreenManager>("SetUpMenu", typeof(MenuPatch), "Menu_Prefix");
			ModHelper.Console.WriteLine("MenuTry ready!");
			ModHelper.Events.AddEvent<Menu>(Events.AfterAwake);
			ModHelper.Events.OnEvent += OnEvent;
			//mainmenu = GameObject.Find("MainMenuLayoutGroup");
			/*GameObject setbut = GameObject.Find("Button-Options");
			extrabut = Object.Instantiate(setbut);
			extrabut.transform.SetParent(mainmenu.transform,false);
			extrabut.GetComponentInChildren<Text>().text = "newtext";
			mainmenu.GetComponentInChildren<VerticalLayoutGroup>().SetLayoutVertical();
			ModHelper.Console.WriteLine("New button added!");*/

		}
		private void OnEvent(MonoBehaviour behaviour, Events ev)
		{
			GameObject cobj = behaviour.gameObject;
			if (cobj.name == "PauseMenuItems")
				cobj = cobj.transform.GetChild(0).gameObject;
			if (cobj.name == "MainMenuLayoutGroup"|| cobj.name == "PauseMenuItemsLayout")
			{
				mainmenu = cobj;
				GameObject setbut = mainmenu.transform.Find("Button-Options").gameObject;
				if (setbut)
				{
					setbut.GetComponentInChildren<Text>().text = "newtext";
					if (extrabut)
					{
						extrabut.GetComponentInChildren<Text>().text = "newtext";
						if (!extrabut.activeSelf)
						{
							extrabut.SetActive(true);
							mainmenu.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
							mainmenu.GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
						}
					}
					else
					{
						if (mainmenu)
						{
							extrabut = GameObject.Instantiate(setbut);
							extrabut.transform.SetParent(mainmenu.transform, false);
							extrabut.transform.position = setbut.transform.position;
							extrabut.GetComponentInChildren<Text>().text = "newtext";
							mainmenu.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputHorizontal();
							mainmenu.GetComponent<VerticalLayoutGroup>().SetLayoutHorizontal();
							ModHelper.Console.WriteLine("New button re-added!");
						}
					}
				}
			}
			ModHelper.Console.WriteLine("Behaviour name: " + behaviour.name);
			if (behaviour.GetType() == typeof(Flashlight) && ev == Events.AfterStart)
			{
				ModHelper.Console.WriteLine("Flashlight has started!");
			}
		}

		private void OnGUI()
		{
			//foreach (Text text in mainmenu.transform.GetComponentsInChildren<Text>())
			//	text.text = "newtext";
			if (extrabut != null)
				extrabut.GetComponentInChildren<Text>().text = "newtext";
			if (setbut != null)
				setbut.GetComponentInChildren<Text>().text = "newset";
			if (extrabut != null && setbut != null)
				extrabut.transform.position = setbut.transform.position;
		}
			
	}
}
