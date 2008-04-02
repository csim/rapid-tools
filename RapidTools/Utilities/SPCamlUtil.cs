using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint;

namespace RapidTools.Utilities
{
	public static class SPCamlUtil
	{

		public static string GetComparison(string comparisonOperator, Guid fieldRefId, string valueType, string value)
		{

			return string.Format("<{0}><FieldRef ID=\"{1}\" /><Value Type=\"{2}\">{3}</Value></{0}>"
				, comparisonOperator, fieldRefId, valueType, value);

		}

		public static string GetComparison(string comparisonOperator, string fieldRefName, string valueType, string value)
		{

			return string.Format("<{0}><FieldRef Name=\"{1}\" /><Value Type=\"{2}\">{3}</Value></{0}>"
				, comparisonOperator, fieldRefName, valueType, value);

		}


		public static string AppendCondition(string previousConditions, string comparisonOperator, string additionalCondition)
		{
			return string.Format("<{1}>{2}{0}</{1}>", previousConditions, comparisonOperator, additionalCondition);
		}


		public static string GetIsNotNullComparison(string fieldRefName)
		{
			return string.Format("<IsNotNull><FieldRef Name=\"{0}\" /></IsNotNull>", fieldRefName);
		}

		public static string GetIsNotNullComparison(Guid fieldRefID)
		{
			return string.Format("<IsNotNull><FieldRef ID=\"{0}\" /></IsNotNull>", fieldRefID);
		}

		public static string GetIsNullComparison(string fieldRefName)
		{
			return string.Format("<IsNull><FieldRef Name=\"{0}\" /></IsNull>", fieldRefName);
		}

		public static string GetIsNullComparison(Guid fieldRefID)
		{
			return string.Format("<IsNull><FieldRef ID=\"{0}\" /></IsNull>", fieldRefID);
		}



	}
}
