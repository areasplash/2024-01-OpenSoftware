using System.Collections.Generic;





public static class Database {





	public enum Unit {
		gram,
		ml,
		piece
	}
	public static string ToString(Unit unit) {
		switch (unit) {
			case Unit.gram:  return "g";
			case Unit.ml:    return "ml";
			case Unit.piece: return "개";
			default:         return "";
		}
	}
	public struct Food {
		public string name;
		public float  amount;
		public Unit   unit;
		public float  price;
		public Food(string name, float amount, Unit unit, float price) {
			this.name   = name;
			this.amount = amount;
			this.unit   = unit;
			this.price  = price;
		}
	}
	public readonly static List<Food> food = new List<Food>{
		new Food("김치", 100.0f, Unit.gram,  1000.0f),
		new Food("대파", 100.0f, Unit.piece, 1000.0f),
		new Food("두부", 100.0f, Unit.piece, 1000.0f),
		new Food("계란", 100.0f, Unit.piece, 1000.0f),
		new Food("소금", 100.0f, Unit.gram,  1000.0f),
	};





	public enum RecipeType {
		Korean,
		Chinese,
		Japanese,
		Western
	}
	public struct Foodset {
		public Food  food;
		public float amount;
		public Foodset(string name, float amount) {
			this.food = Database.food.Find(x => x.name == name);
			this.amount = amount;
		}
	}
	public struct Recipe {
		public string        name;
		public RecipeType    type;
		public List<Foodset> foodset;
		public Recipe(string name, RecipeType type, List<Foodset> foodset) {
			this.name    = name;
			this.type    = type;
			this.foodset = foodset;
		}
	}
	public readonly static List<Recipe> recipe = new List<Recipe>{
		new Recipe("김치찌개", RecipeType.Korean, new List<Foodset>{
			new Foodset("김치", 100.0f),
			new Foodset("대파",   1.0f),
			new Foodset("두부",   1.0f),
		}),
		new Recipe("계란말이", RecipeType.Korean, new List<Foodset>{
			new Foodset("계란",   4.0f),
			new Foodset("대파",   1.0f),
			new Foodset("소금",   1.0f),
		}),
	};





	public struct Menuset {
		public Recipe recipe;
		public float  price;
		public Menuset(string name, float price) {
			this.recipe = Database.recipe.Find(x => x.name == name);
			this.price  = price;
		}
	}
	public struct Restaurant {
		public string        name;
		public string        url;
		public List<Menuset> menuset;
		public Restaurant(string name, List<Menuset> menuset) {
			this.name    = name;
			this.url = "https://map.naver.com/p/search/" + name;
			this.menuset = menuset;
		}
	}
	public readonly static List<Restaurant> restaurant = new List<Restaurant>{
		new Restaurant("김치만선생 동대필동점", new List<Menuset>{
			new Menuset("김치찌개", 7000.0f),
			new Menuset("계란말이", 6000.0f),
		}),
	};
}
