using RealtyCloudAPI;
using System;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Test
{
	class Program
	{
		private const string API_KEY = "ENTER_API_KEY_HERE";

		static void Main(string[] args)
		{
			string address = "ENTER_ADDRESS_HERE";

			var noKeyRequest = new RealtyRequest();

			var info = noKeyRequest.Search(address);
			Console.WriteLine(ToXmlString(info));
			Thread.Sleep(1000);

			noKeyRequest.SearchAsync(address, s => Console.WriteLine(ToXmlString(s)), e => Console.WriteLine(e));
			Thread.Sleep(1000);

			var keyRequest = new RealtyRequest(API_KEY);

			var request = new OrderRequest(false, new OrderRequestItem("EgrnRightList", info[0].CadastralNumber));

			var order = keyRequest.CreateOrder(request);
			Console.WriteLine(ToXmlString(order));
			Thread.Sleep(1000);

			Console.WriteLine(ToXmlString(keyRequest.OrdersStatus(order.Items[0].OrderItemID)));
			Thread.Sleep(1000);

			Console.WriteLine(ToXmlString(keyRequest.GetObjectInfo(info[0].CadastralNumber)));
			Thread.Sleep(1000);

			Console.WriteLine(ToXmlString(keyRequest.ProductsList()));
			Thread.Sleep(1000);

			keyRequest.CreateOrderAsync(request, s => Console.WriteLine(ToXmlString(s)), e => Console.WriteLine(e));
			Thread.Sleep(1000);

			keyRequest.OrdersStatusAsync(new string[] { order.Items[0].OrderItemID }, s => Console.WriteLine(ToXmlString(s)), e => Console.WriteLine(e));
			Thread.Sleep(1000);

			keyRequest.GetObjectInfoAsync(info[0].CadastralNumber, s => Console.WriteLine(ToXmlString(s)), e => Console.WriteLine(e));
			Thread.Sleep(1000);

			keyRequest.ProductsListAsync(s => Console.WriteLine(ToXmlString(s)), e => Console.WriteLine(e));
			Thread.Sleep(20000);
		}

		private static string ToXmlString<T>(T value)
		{
			StringBuilder sb = new StringBuilder();
			var writer = XmlWriter.Create(sb, new XmlWriterSettings() {
				Indent = true
			});
			new XmlSerializer(typeof(T)).Serialize(writer, value);
			return sb.ToString();
		}
	}
}