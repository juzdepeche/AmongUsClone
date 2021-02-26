using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildHelper
{
	public static string GetArg(BuildParameter argument)
	{
		string argumentName = GetArgumentName(argument);
		if (argumentName == string.Empty) return null;

		var args = System.Environment.GetCommandLineArgs();
		for (int i = 0; i < args.Length; i++)
		{
			if (args[i].ToLower() == argumentName.ToLower() && args.Length >= i + 1)
			{
				return args[i + 1];
			}
		}
		return null;
	}

	private static string GetArgumentName(BuildParameter argument)
	{
		switch (argument)
		{
			case BuildParameter.Username:
				return "username";
			case BuildParameter.GoTo:
				return "goTo";
		}

		return string.Empty;
	}
}

public enum BuildParameter
{
	Username,
	GoTo
}

