using System.Collections.Generic;

namespace RealtyCloudAPI
{
	/// <summary>
	/// Информация о продукте.
	/// </summary>
	public struct ProductInfo//TODO: доступные плюшки с тарифом
	{
		/// <summary>
		/// Уникальный идентификатор продукта.
		/// </summary>
		public string ID;
		/// <summary>
		/// Имя (ключ) продукта.
		/// </summary>
		public string Name;
		/// <summary>
		/// Название продукта.
		/// </summary>
		public string Title;
		/// <summary>
		/// Описание продукта.
		/// </summary>
		public string Description;
		/// <summary>
		/// Цена продукта.
		/// </summary>
		public decimal Price;
		/// <summary>
		/// Тип продукта.
		/// </summary>
		public string Type;
		/// <summary>
		/// Активен ли данный продукт.
		/// </summary>
		public bool IsActive;

		internal static bool TryBuild(IDictionary<string, object> data, out ProductInfo value)
		{
			value = new ProductInfo();

			if(!data.TryGetString("product_id", out value.ID))
			{
				return false;
			}

			if(!data.TryGetString("product_name", out value.Name))
			{
				return false;
			}

			if(!data.TryGetString("product_name_ru", out value.Title))
			{
				return false;
			}

			if(!data.TryGetString("product_description", out value.Description))
			{
				return false;
			}

			if(!data.TryGetDecimal("product_price", out value.Price))
			{
				return false;
			}

			if(!data.TryGetString("product_type", out value.Type))
			{
				return false;
			}

			if(!data.TryGetBool("product_active", out value.IsActive))
			{
				return false;
			}

			return true;
		}
	}
}
