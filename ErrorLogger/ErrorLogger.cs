using OWML.Common;
using OWML.ModHelper;
using OWML.ModHelper.Events;
using OWML.ModLoader;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using System.Security.AccessControl;
using System.Collections.Generic;

namespace OW_ErrorLogger
{
	public class ErrorLogger : ModBehaviour
	{
		private void Start()
		{
			Application.logMessageReceived += OnLogMessageReceived;
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
