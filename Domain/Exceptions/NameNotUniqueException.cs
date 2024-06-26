﻿namespace Domain.Exceptions;

public class NameNotUniqueException : Exception
{
	public NameNotUniqueException()
	{
	}

	public NameNotUniqueException(string message)
		: base(message)
	{
	}

	public NameNotUniqueException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
