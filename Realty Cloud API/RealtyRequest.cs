using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RealtyCloudAPI
{
	public class RealtyRequest
	{
		public delegate void AsyncResponseCallback<T>(T value);
		public delegate void AsyncResponseError(string msg);

		private const string SERVER = "https://api.realtycloud.ru";

		/// <summary>
		/// Сертификат для проверки сервера, при отправке запросов.
		/// </summary>
		private X509Certificate certificate = null;
		/// <summary>
		/// Ключ API, если задан - отправляется вместе с запросом.
		/// </summary>
		private string apiKey = null;

		public RealtyRequest(X509Certificate certificate = null)
		{
			this.apiKey = null;
			this.certificate = certificate;
		}

		public RealtyRequest(string apiKey, X509Certificate certificate = null)
		{
			this.apiKey = apiKey;
			this.certificate = certificate;
		}

		/// <summary>
		/// Поиск объектов по адресу.
		/// </summary>
		/// <returns>Список объектов или null (в случае непредвиденных ошибок)</returns>
		public ObjectBaseInfo[] Search(string target)
		{
			var request = new Request(new string[] {
				SERVER,
				"search"
			}, new string[,] {
				{ "query", target }
			}, certificate);

			if(!string.IsNullOrEmpty(apiKey)) request.SetAPIKey(apiKey);

			object obj = request.GETRequest();
			ObjectBaseInfo[] value;
			if(obj is IDictionary<string, object> && (obj as IDictionary<string, object>).TryGetObjArray("data", ObjectBaseInfo.TryBuild, out value))
			{
				return value;
			}

			System.Diagnostics.Debug.WriteLine("Invalid JSON structure");
			return null;
		}
		/// <summary>
		/// Асинхронный поиск объектов по адресу.
		/// </summary>
		/// <param name="callback">Вызывается с результатом, в случае успеха.</param>
		/// <param name="onError">Вызывается в случае ошибки</param>
		public void SearchAsync(string target, AsyncResponseCallback<ObjectBaseInfo[]> callback, AsyncResponseError onError = null)
		{
			if(callback == null) throw new ArgumentNullException("callback");

			var request = new Request(new string[] {
				SERVER,
				"search"
			}, new string[,] {
				{ "query", target }
			}, certificate);

			if(!string.IsNullOrEmpty(apiKey)) request.SetAPIKey(apiKey);

			var requestInfo = new AsyncArrayHandler<ObjectBaseInfo>(ObjectBaseInfo.TryBuild, callback, onError);
			request.GETRequestAsync(requestInfo.OnData);
		}

		/// <summary>
		/// Создание заказа на продукт(ы).
		/// </summary>
		/// <returns>Информация о запросе или значение по умолчанию (в случае непредвиденных ошибок)</returns>
		public OrderInfo CreateOrder(OrderRequest order)
		{
			var request = new Request(new string[] {
				SERVER,
				"order"
			}, new string[,] { }, certificate);

			if(!string.IsNullOrEmpty(apiKey)) request.SetAPIKey(apiKey);

			object obj = request.POSTRequest(Encoding.UTF8.GetBytes(JSON.ToString(order.ToObject())));
			OrderInfo value;
			if(obj is IDictionary<string, object> && (obj as IDictionary<string, object>).TryGetObj("data", OrderInfo.TryBuild, out value))
			{
				return value;
			}

			System.Diagnostics.Debug.WriteLine("Invalid JSON structure");
			return default(OrderInfo);
		}
		/// <summary>
		/// Асинхронное создание запроса продукты.
		/// </summary>
		/// <param name="callback">Вызывается с результатом, в случае успеха.</param>
		/// <param name="onError">Вызывается в случае ошибки</param>
		public void CreateOrderAsync(OrderRequest order, AsyncResponseCallback<OrderInfo> callback, AsyncResponseError onError = null)
		{
			if(callback == null) throw new ArgumentNullException("callback");

			var request = new Request(new string[] {
				SERVER,
				"order"
			}, new string[,] { }, certificate);

			if(!string.IsNullOrEmpty(apiKey)) request.SetAPIKey(apiKey);

			var requestInfo = new AsyncObjectHandler<OrderInfo>(OrderInfo.TryBuild, callback, onError);
			request.POSTRequestAsync(Encoding.UTF8.GetBytes(JSON.ToString(order.ToObject())), requestInfo.OnData);
		}

		/// <summary>
		/// Получение статуса заказов.
		/// </summary>
		/// <returns>Список статусов или null (в случае непредвиденных ошибок)</returns>
		public OrderStatusInfo[] OrdersStatus(params string[] ids)
		{
			var list = new string[ids.Length, 2];

			if(ids.Length > 1)
			{
				for(int i = 0; i < ids.Length; i++)
				{
					list[i, 0] = "orderItemID[]";
					list[i, 1] = ids[i];
				}
			}
			else
			{
				list[0, 0] = "orderItemID";
				list[0, 1] = ids[0];
			}

			var request = new Request(new string[] {
				SERVER,
				"orders"
			}, list, certificate);

			if(!string.IsNullOrEmpty(apiKey)) request.SetAPIKey(apiKey);

			object obj = request.GETRequest();
			OrderStatusInfo[] value;
			if(obj is IDictionary<string, object> && (obj as IDictionary<string, object>).TryGetObjArray("data", OrderStatusInfo.TryBuild, out value))
			{
				return value;
			}

			System.Diagnostics.Debug.WriteLine("Invalid JSON structure");
			return null;
		}
		/// <summary>
		/// Асинхронное получение статуса заказов.
		/// </summary>
		/// <param name="callback">Вызывается с результатом, в случае успеха.</param>
		/// <param name="onError">Вызывается в случае ошибки</param>
		public void OrdersStatusAsync(string[] ids, AsyncResponseCallback<OrderStatusInfo[]> callback, AsyncResponseError onError = null)
		{
			if(callback == null) throw new ArgumentNullException("callback");

			var list = new string[ids.Length, 2];

			if(ids.Length > 1)
			{
				for(int i = 0; i < ids.Length; i++)
				{
					list[i, 0] = "orderItemID[]";
					list[i, 1] = ids[i];
				}
			}
			else
			{
				list[0, 0] = "orderItemID";
				list[0, 1] = ids[0];
			}

			var request = new Request(new string[] {
				SERVER,
				"orders"
			}, list, certificate);

			if(!string.IsNullOrEmpty(apiKey)) request.SetAPIKey(apiKey);

			var requestInfo = new AsyncArrayHandler<OrderStatusInfo>(OrderStatusInfo.TryBuild, callback, onError);
			request.GETRequestAsync(requestInfo.OnData);
		}

		/// <summary>
		/// Получение информации об объекте.
		/// </summary>
		/// <returns>Информация об объекте или значение по умолчанию (в случае непредвиденных ошибок)</returns>
		public ObjectData GetObjectInfo(string cadastralNumber)
		{
			var request = new Request(new string[] {
				SERVER,
				"object",
				cadastralNumber
			}, new string[,] { }, certificate);

			if(!string.IsNullOrEmpty(apiKey)) request.SetAPIKey(apiKey);

			object obj = request.GETRequest();
			ObjectData value;
			if(obj is IDictionary<string, object> && (obj as IDictionary<string, object>).TryGetObj("data", ObjectData.TryBuild, out value))
			{
				return value;
			}

			System.Diagnostics.Debug.WriteLine("Invalid JSON structure");
			return default(ObjectData);
		}
		/// <summary>
		/// Асинхронное получение информации об объекте.
		/// </summary>
		/// <param name="callback">Вызывается с результатом, в случае успеха.</param>
		/// <param name="onError">Вызывается в случае ошибки</param>
		public void GetObjectInfoAsync(string cadastralNumber, AsyncResponseCallback<ObjectData> callback, AsyncResponseError onError = null)
		{
			if(callback == null) throw new ArgumentNullException("callback");

			var request = new Request(new string[] {
				SERVER,
				"object",
				cadastralNumber
			}, new string[,] { }, certificate);

			if(!string.IsNullOrEmpty(apiKey)) request.SetAPIKey(apiKey);

			var requestInfo = new AsyncObjectHandler<ObjectData>(ObjectData.TryBuild, callback, onError);
			request.GETRequestAsync(requestInfo.OnData);
		}

		/// <summary>
		/// Получение списка продуктов.
		/// </summary>
		/// <returns>Список продуктов или null (в случае непредвиденных ошибок)</returns>
		public ProductInfo[] ProductsList()
		{
			var request = new Request(new string[] {
				SERVER,
				"products"
			}, new string[,] { }, certificate);

			if(!string.IsNullOrEmpty(apiKey)) request.SetAPIKey(apiKey);

			object obj = request.GETRequest();
			ProductInfo[] value;
			if(obj is IDictionary<string, object> && (obj as IDictionary<string, object>).TryGetObjArray("data", ProductInfo.TryBuild, out value))
			{
				return value;
			}

			System.Diagnostics.Debug.WriteLine("Invalid JSON structure");
			return null;
		}
		/// <summary>
		/// Асинхронное получение списка продуктов.
		/// </summary>
		/// <param name="callback">Вызывается с результатом, в случае успеха.</param>
		/// <param name="onError">Вызывается в случае ошибки</param>
		public void ProductsListAsync(AsyncResponseCallback<ProductInfo[]> callback, AsyncResponseError onError = null)
		{
			if(callback == null) throw new ArgumentNullException("callback");

			var request = new Request(new string[] {
				SERVER,
				"products"
			}, new string[,] { }, certificate);

			if(!string.IsNullOrEmpty(apiKey)) request.SetAPIKey(apiKey);

			var requestInfo = new AsyncArrayHandler<ProductInfo>(ProductInfo.TryBuild, callback, onError);
			request.GETRequestAsync(requestInfo.OnData);
		}

		private class AsyncArrayHandler<T>
		{
			private AsyncResponseCallback<T[]> callback;
			private Utility.TryObjectBuild<T> builder;
			private AsyncResponseError onError;

			public AsyncArrayHandler(Utility.TryObjectBuild<T> builder, AsyncResponseCallback<T[]> callback, AsyncResponseError onError)
			{
				this.callback = callback;
				this.onError = onError;
				this.builder = builder;
			}

			public void OnData(bool success, object obj, string msg)
			{
				if(!success || obj == null)
				{
					if(onError != null) onError.Invoke(msg);
				}
				else
				{
					T[] value;
					if(obj is IDictionary<string, object> && (obj as IDictionary<string, object>).TryGetObjArray("data", builder, out value))
					{
						callback.Invoke(value);
					}
					else
					{
						System.Diagnostics.Debug.WriteLine("Invalid JSON structure");
						if(onError != null) onError.Invoke("Invalid JSON structure");
					}
				}
			}
		}
		private class AsyncObjectHandler<T>
		{
			private AsyncResponseCallback<T> callback;
			private Utility.TryObjectBuild<T> builder;
			private AsyncResponseError onError;

			public AsyncObjectHandler(Utility.TryObjectBuild<T> builder, AsyncResponseCallback<T> callback, AsyncResponseError onError)
			{
				this.callback = callback;
				this.onError = onError;
				this.builder = builder;
			}

			public void OnData(bool success, object obj, string msg)
			{
				if(!success || obj == null)
				{
					if(onError != null) onError.Invoke(msg);
				}
				else
				{
					T value;
					if(obj is IDictionary<string, object> && (obj as IDictionary<string, object>).TryGetObj("data", builder, out value))
					{
						callback.Invoke(value);
					}
					else
					{
						System.Diagnostics.Debug.WriteLine("Invalid JSON structure");
						if(onError != null) onError.Invoke("Invalid JSON structure");
					}
				}
			}
		}
	}
}