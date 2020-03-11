using System.Collections.Generic;

namespace RealtyCloudAPI
{
	/// <summary>
	/// Базовая информация об объекте.
	/// </summary>
	public struct ObjectBaseInfo
	{
		/// <summary>
		/// Кадастровый номер объекта.
		/// </summary>
		public string CadastralNumber;
		/// <summary>
		/// Адрес объекта.
		/// </summary>
		public string Address;
		/// <summary>
		/// Тип объекта.
		/// </summary>
		public string ObjectType;
		/// <summary>
		/// Площадь объекта.
		/// </summary>
		public decimal Area;
		/// <summary>
		/// Кадастровая стоимость объекта.
		/// </summary>
		public decimal CadastralPrice;
		/// <summary>
		/// Статус объекта.
		/// </summary>
		public string Status;

		internal static bool TryBuild(IDictionary<string, object> data, out ObjectBaseInfo value)
		{
			value = new ObjectBaseInfo();

			if(!data.TryGetString("Number", out value.CadastralNumber))
			{
				return false;
			}

			if(!data.TryGetString("Address", out value.Address))
			{
				return false;
			}

			data.TryGetString("ObjectType", out value.ObjectType);
			data.TryGetDecimal("kad_price", out value.CadastralPrice);
			data.TryGetString("Status", out value.Status);
			data.TryGetDecimal("Area", out value.Area);

			return true;
		}
	}
}