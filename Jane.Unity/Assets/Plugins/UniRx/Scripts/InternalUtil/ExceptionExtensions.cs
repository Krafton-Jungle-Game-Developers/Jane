using System;

namespace UniRx.InternalUtil
{

	internal static class ExceptionExtensions
	{
		public static void Throw(this Exception exception)
		{
			System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception).Throw();

            throw exception;
		}
	}
}
