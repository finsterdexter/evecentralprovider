﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using EveCentralProvider.Types;
using System.Linq;

namespace EveCentralProvider.Tests
{
	[TestClass]
	public class ServicesTests
	{
		private static List<int> RegionId = new List<int>() { 10000014 };
		private static List<int> TypeId = new List<int>() { 34, 35, 36 };

		[TestMethod]
		public void MarketStatTest()
		{
			Services target = Services.Instance;
			List<TypeMarketStats> output = target.MarketStat(TypeId, RegionId);

			Assert.IsTrue(output.Count == 3);
			Assert.IsTrue(output[0].Id == 34);
			Assert.IsTrue(output[1].Id == 35);
			Assert.IsTrue(output[2].Id == 36);

		}

		[TestMethod]
		public void QuickLookTest()
		{
			Services target = Services.Instance;
			QuickLookResult output = target.QuickLook(TypeId[0], RegionId);

			Assert.IsTrue(output.Item == 34);
			Assert.IsTrue(output.ItemName == "Tritanium");
			Assert.IsTrue(output.BuyOrders.Count > 0);
			Assert.IsTrue(output.SellOrders.Count > 0);

		}

		[TestMethod]
		public void QuickLookPathTest()
		{
			Services target = Services.Instance;
			QuickLookPathResult output = target.QuickLookPath("Jita", "Amarr", 34);

			Assert.IsTrue(output.Item == 34);
			Assert.IsTrue(output.ItemName == "Tritanium");
			Assert.IsTrue(output.BuyOrders.Count > 0);
			Assert.IsTrue(output.SellOrders.Count > 0);
			Assert.IsTrue(output.From == 30000142);
			Assert.IsTrue(output.To == 30002187);
		}

		[TestMethod]
		public void HistoryTest()
		{
			Services target = Services.Instance;
			List<TypeHistory> output = target.History(34, LocaleType.System, "Amarr", OrderType.Sell);

			Assert.IsTrue(output.Count > 0);
			Assert.IsTrue(output[0].Volume > 0);
		}

		[TestMethod]
		public void EveMonTest()
		{
			Services target = Services.Instance;
			EveMonResult output = target.EveMon();

			Assert.IsTrue(output.Minerals.First().name == "Tritanium");
		}

		[TestMethod]
		public void RouteTest()
		{
			Services target = Services.Instance;
			List<RouteJump> output = target.Route("Jita", "HED-GP");

			
		}
	}
}
