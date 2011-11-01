﻿using System.ComponentModel;
using System.Linq;
using FluentValidation.Results;
using Talifun.Commander.Command.Configuration;

namespace Talifun.Commander.Command.YouTubeUploader.Configuration
{
	public partial class YouTubeUploaderElement : IDataErrorInfo
	{
		public bool IsValid
		{
			get { return SelfValidate().IsValid; }
		}

		private ValidationResult SelfValidate()
		{
			return ValidationHelper.Validate<YouTubeUploaderElementValidator, YouTubeUploaderElement>(this);
		}

		string IDataErrorInfo.Error
		{
			get { return ValidationHelper.GetError(SelfValidate()); }
		}

		string IDataErrorInfo.this[string columnName]
		{
			get
			{
				var validationResults = SelfValidate();
				if (validationResults == null) return string.Empty;
				var columnResults = validationResults.Errors.FirstOrDefault<ValidationFailure>(x => string.Compare(x.PropertyName, columnName, true) == 0);
				return columnResults != null ? columnResults.ErrorMessage : string.Empty;
			}
		}
	}
}
