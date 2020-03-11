using System;
using System.Collections.Generic;

namespace RealtyCloudAPI
{
	public struct ObjectData
	{
		/// <summary>
		/// Информация об объекте.
		/// </summary>
		public ObjectInfo Info;
		/// <summary>
		/// Список заказанных продуктов с этим объектом.
		/// </summary>
		public OrderItem[] Products;

		internal static bool TryBuild(IDictionary<string, object> data, out ObjectData value)
		{
			value = new ObjectData();

			if(!data.TryGetObj("object", ObjectInfo.TryBuild, out value.Info))
			{
				return false;
			}

			data.TryGetObjArray("products", OrderItem.TryBuild, out value.Products);

			return true;
		}
	}

	/// <summary>
	/// Полная информация об объекте.
	/// </summary>
	public struct ObjectInfo
	{
		/// <summary>
		/// Наименование объекта.
		/// </summary>
		public string Name;
		/// <summary>
		/// Дополнительная информация об объекте.
		/// </summary>
		public string Info;
		/// <summary>
		/// Тип объекта.
		/// </summary>
		public string ObjectType;
		/// <summary>
		/// Кадастровый номер
		/// </summary>
		public string CadastralNumber;
		/// <summary>
		/// Адрес объекта.
		/// </summary>
		public string Address;
		/// <summary>
		/// Регион или код региона.
		/// </summary>
		public string Region;
		/// <summary>
		/// Разрешенное использование объекта.
		/// </summary>
		public string PermittedUse;
		/// <summary>
		/// Форма собственности.
		/// </summary>
		public string OwnershipForm;
		/// <summary>
		/// Вид земель.
		/// </summary>
		public string LandCategory;
		/// <summary>
		/// Количество владельцев.
		/// Если владельцы отсутствуют - выставляется значение -1.
		/// </summary>
		public int OwnersCount;
		/// <summary>
		/// Номер этажа.
		/// </summary>
		public int Level;
		/// <summary>
		/// Объект аннулирован.
		/// </summary>
		public bool IsCanceled;

		/// <summary>
		/// Площадь объекта.
		/// </summary>
		public decimal Area;
		/// <summary>
		/// Кадастровая стоимость объекта.
		/// </summary>
		public decimal KadPrice;
		/// <summary>
		/// Кадастровый инженер.
		/// </summary>
		public string KadEngineer;
		/// <summary>
		/// Дата постановки на кадастровый учет.
		/// </summary>
		public DateTime KadRegisterDate;
		/// <summary>
		/// Площадь ОКС.
		/// </summary>
		public decimal AreaCCO;
		/// <summary>
		/// Тип ОКС.
		/// </summary>
		public string TypeCCO;

		/// <summary>
		/// Дата учета объекта.
		/// </summary>
		public DateTime RecordDate;
		/// <summary>
		/// Время обновления объекта.
		/// </summary>
		public DateTime UpdateTime;
		public DateTime Date;
		/// <summary>
		/// Дата внесения стоимости.
		/// </summary>
		public DateTime AddPriceDate;
		/// <summary>
		/// Дата утверждения стоимости.
		/// </summary>
		public DateTime ApprovePriceDate;
		/// <summary>
		/// Дата назначения стоимости.
		/// </summary>
		public DateTime AssignPriceDate;

		internal static bool TryBuild(IDictionary<string, object> data, out ObjectInfo value)
		{
			value = new ObjectInfo();
			
			if(!data.TryGetString("Number", out value.CadastralNumber))
			{
				return false;
			}

			if(!data.TryGetString("Address", out value.Address))
			{
				return false;
			}

			if(!data.TryGetDecimal("Area", out value.Area))
			{
				return false;
			}

			data.TryGetString("Name", out value.Name);
			data.TryGetString("Info", out value.Info);
			data.TryGetString("ObjectType", out value.ObjectType);

			data.TryGetString("Region", out value.Region);
			data.TryGetString("PermittedUse", out value.PermittedUse);
			data.TryGetString("FormOfOwnership", out value.OwnershipForm);
			data.TryGetString("LandCategory", out value.LandCategory);
			if(!data.TryGetInt("OwnersCount", out value.OwnersCount)) value.OwnersCount = -1;
			data.TryGetInt("Level", out value.Level);
			data.TryGetBool("IsCanceled", out value.IsCanceled);

			data.TryGetDecimal("KadPrice", out value.KadPrice);
			data.TryGetString("KadEngineer", out value.KadEngineer);
			data.TryGetDateTime("DateOfKadReg", out value.KadRegisterDate);

			data.TryGetDecimal("AreaOKC", out value.AreaCCO);
			data.TryGetString("TypeOKC", out value.TypeCCO);

			data.TryGetDateTime("YearBuilt", out value.RecordDate);
			data.TryGetDateTime("UpdatedAt", out value.UpdateTime);
			data.TryGetDateTime("Date", out value.Date);
			data.TryGetDateTime("DateOfPriceAdded", out value.AddPriceDate);
			data.TryGetDateTime("PriceApprovalDate", out value.ApprovePriceDate);
			data.TryGetDateTime("PriceDeterminationDate", out value.AssignPriceDate);

			return true;
		}
	}
}