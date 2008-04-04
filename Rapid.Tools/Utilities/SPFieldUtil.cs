using System;
using System.Data;
using System.Configuration;
using System.Web;
using Microsoft.SharePoint;

namespace Rapid.Tools.Utilities
{
	public static class SPFieldUtil
	{

		public static bool FieldExists(SPListItem item, Guid fieldID)
		{
			try
			{
				SPField field = item.Fields[fieldID];
				return true;
			}
			catch (ArgumentException)
			{
				return false;
			}
		}

		public static bool FieldExists(SPListItem item, string fieldname)
		{
			try
			{
				SPField field = item.Fields.GetFieldByInternalName(fieldname);
				return true;
			}
			catch (ArgumentException)
			{
				return false;
			}
		}

		public static bool FieldExists(SPList list, Guid fieldID)
		{
			try
			{
				SPField field = list.Fields[fieldID];
				return true;
			}
			catch (ArgumentException)
			{
				return false;
			}
		}

		public static bool FieldExists(SPList list, string fieldname)
		{
			try
			{
				SPField field = list.Fields.GetFieldByInternalName(fieldname);
				return true;
			}
			catch (ArgumentException)
			{
				return false;
			}
		}


		public static SPField GetField(SPListItem item, Guid fieldID)
		{
			bool exists = FieldExists(item, fieldID);
			return exists ? item.Fields[fieldID] : null;
		}

		public static SPField GetField(SPListItem item, string fieldname)
		{
			bool exists = FieldExists(item, fieldname);
			return exists ? item.Fields.GetFieldByInternalName(fieldname) : null;
		}

		public static SPField GetField(SPList list, Guid fieldID)
		{
			bool exists = FieldExists(list, fieldID);
			return exists ? list.Fields[fieldID] : null;
		}

		public static SPField GetField(SPList list, string fieldname)
		{
			bool exists = FieldExists(list, fieldname);
			return (exists) ? list.Fields.GetFieldByInternalName(fieldname) : null;

		}


		public static object GetFieldValue(SPListItem item, Guid fieldID)
		{
			bool exists = FieldExists(item, fieldID);
			return (exists) ? item[fieldID] : null;
		}

		public static object GetFieldValue(SPListItem item, string fieldname)
		{
			bool exists = FieldExists(item, fieldname);
			return (exists) ? item[fieldname] : null;
		}


		public static T GetFieldValue<T>(SPListItem item, string fieldname)
		{
			return GetFieldValue(item, fieldname, default(T));
		}

		public static T GetFieldValue<T>(SPListItem item, string fieldname, T defaultValue)
		{
			try
			{
				return (T)item[fieldname];
			}
			catch { }

			return defaultValue;
		}


		public static void SetFieldValue(SPListItem item, Guid fieldID, object value)
		{
			bool exists = FieldExists(item, fieldID);

			if (exists)
				item[fieldID] = value;

		}

		public static void SetFieldValue(SPListItem item, string fieldname, object value)
		{
			bool exists = FieldExists(item, fieldname);

			if (exists)
				item[fieldname] = value;

		}


		public static bool TryGetField(SPListItem item, Guid fieldID, out SPField field)
		{
			bool exists = FieldExists(item, fieldID);

			if (exists)
			{
				field = item.Fields[fieldID];
				return true;
			}
			else
			{
				field = null;
				return false;
			}
		}

		public static bool TryGetField(SPListItem item, string fieldname, out SPField field)
		{
			bool exists = FieldExists(item, fieldname);

			if (exists)
			{
				field = item.Fields[fieldname];
				return true;
			}
			else
			{
				field = null;
				return false;
			}
		}

		public static bool TryGetField(SPList list, Guid fieldID, SPField field)
		{

			bool exists = FieldExists(list, fieldID);

			if (exists)
			{
				field = list.Fields[fieldID];
				return true;
			}
			else
			{
				field = null;
				return false;
			}

		}

		public static bool TryGetField(SPList list, string fieldname, SPField field)
		{

			bool exists = FieldExists(list, fieldname);

			if (exists)
			{
				field = list.Fields[fieldname];
				return true;
			}
			else
			{
				field = null;
				return false;
			}

		}


		public static bool TryGetFieldValue(SPListItem item, Guid fieldID, out object value)
		{

			bool exists = FieldExists(item, fieldID);

			if (exists)
			{
				value = item[fieldID];
				return true;
			}
			else
			{
				value = null;
				return false;
			}

		}

		public static bool TryGetFieldValue(SPListItem item, string fieldname, out object value)
		{

			bool exists = FieldExists(item, fieldname);

			if (exists)
			{
				value = item[fieldname];
				return true;
			}
			else
			{
				value = null;
				return false;
			}

		}

		public static bool TrySetFieldValue(SPListItem item, Guid fieldID, object value)
		{

			try
			{
				bool exists = FieldExists(item, fieldID);

				if (exists)
				{
					item[fieldID] = value;
					return true;
				}
				else
				{
					return false;
				}
			}
			catch
			{
				return false;
			}
		}

		public static bool TrySetFieldValue(SPListItem item, string fieldname, object value)
		{

			try
			{
				bool exists = FieldExists(item, fieldname);

				if (exists)
				{
					item[fieldname] = value;
					return true;
				}
				else
				{
					return false;
				}
			}
			catch
			{
				return false;
			}
		}

	}
}
