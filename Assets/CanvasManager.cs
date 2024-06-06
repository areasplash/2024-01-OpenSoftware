using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;





public class CanvasManager : MonoBehaviour {
	RectTransform rect;
	Text text;
	Button button;





	void Start() {
		int foodCount = PlayerPrefs.GetInt("foodCount");
        for (int i = 0; i < foodCount; i++) {
			string name = PlayerPrefs.GetString("name" + i);
			string date = PlayerPrefs.GetString("date" + i);
			DateTime expirationDate = DateTime.ParseExact(date, "yyyy.MM.dd", null);
			FoodObject foodObject = AddFood(name, expirationDate);
			foodList.Add(foodObject);
		}

        InitMain();
		InitRefrigerator();
		InitRecipe();
		InitAnalysis();
		InitStatusBar();
		InitNavigationBar();

		pageMain.gameObject.SetActive(true);
	}

	void Update() {
		if (barStatus.gameObject.activeSelf) textTime.text = DateTime.Now.ToString("HH:mm");
		if (Input.GetKeyDown(KeyCode.Escape)) Back();
	}

	void ChangePageTo(Canvas page) {
		pageMain        .gameObject.SetActive(false);
		pageRefrigerator.gameObject.SetActive(false);
		pageRecipe      .gameObject.SetActive(false);
		pageAnalysis    .gameObject.SetActive(false);
		page            .gameObject.SetActive(true );
	}

	void Back() {
		if (pageMain             .gameObject.activeSelf) {
			#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
			#else
				Application.Quit();
			#endif
		}
		else if (pageRefrigerator.gameObject.activeSelf) {
			ChangePageTo(pageMain);
		}
		else if (pageRecipe      .gameObject.activeSelf) {
			if (rectRecipePopup.gameObject.activeSelf) {
				rectRecipePopup.gameObject.SetActive(false);
				return;
			}
			ChangePageTo(pageMain);
		}
		else if (pageAnalysis     .gameObject.activeSelf) {
			ChangePageTo(pageRecipe);
		}
	}





	[Header("Main")]
	[SerializeField] Canvas pageMain;

	[SerializeField] Button buttonMainToRefrigerator;
	[SerializeField] Button buttonMainToRecipe;

	void InitMain() {
		pageMain.gameObject.SetActive(false);

		buttonMainToRefrigerator.onClick.AddListener(() => {
			ChangePageTo(pageRefrigerator);
		});
		buttonMainToRecipe.onClick.AddListener(() => {
			ChangePageTo(pageRecipe);
		});
	}





	[Header("Refrigerator")]
	[SerializeField] Canvas pageRefrigerator;

	[SerializeField] ScrollRect rectFood;
	[SerializeField] ScrollRect rectFoodDangerous;

	[SerializeField] Button buttonAddFood;
	[SerializeField] Image inputFoodInfo;

	[SerializeField] InputField inputFoodName;
	[SerializeField] InputField inputFoodYear;
	[SerializeField] InputField inputFoodMonth;
	[SerializeField] InputField inputFoodDay;
	[SerializeField] Button buttonChangeInputField;
	[SerializeField] Button buttonSubmitFood;

	[SerializeField] GameObject prefabFood;

	struct FoodObject {
		public GameObject gameObject;
		public string name;
		public DateTime expirationDate;
	}
	List<FoodObject> foodList = new List<FoodObject>();

	void InitRefrigerator() {
		pageRefrigerator.gameObject.SetActive(false);
		buttonAddFood.gameObject.SetActive(true);
		inputFoodInfo.gameObject.SetActive(false);
		RefreshFood();

		buttonAddFood.onClick.AddListener(() => {
			buttonAddFood .gameObject.SetActive(false);
			inputFoodInfo .gameObject.SetActive(true );
			inputFoodName .gameObject.SetActive(true );
			inputFoodYear .gameObject.SetActive(false);
			inputFoodMonth.gameObject.SetActive(false);
			inputFoodDay  .gameObject.SetActive(false);
			inputFoodName .text = "";
			DateTime date = DateTime.Now + new TimeSpan(7, 0, 0, 0);
			inputFoodYear .text = date.ToString("yyyy");
			inputFoodMonth.text = date.ToString(  "MM");
			inputFoodDay  .text = date.ToString(  "dd");
		});
		buttonChangeInputField.onClick.AddListener(() => {
			buttonChangeInputField.transform.GetChild(0).TryGetComponent(out text);
			if (inputFoodName.gameObject.activeSelf) {
				text.text = "식재료명";
				inputFoodName .gameObject.SetActive(false);
				inputFoodYear .gameObject.SetActive(true );
				inputFoodMonth.gameObject.SetActive(true );
				inputFoodDay  .gameObject.SetActive(true );
			}
			else {
				text.text = "유통기한";
				inputFoodName .gameObject.SetActive(true );
				inputFoodYear .gameObject.SetActive(false);
				inputFoodMonth.gameObject.SetActive(false);
				inputFoodDay  .gameObject.SetActive(false);
			}
		});
		buttonSubmitFood.onClick.AddListener(() => {
			if (inputFoodName.text == "") return;
			buttonAddFood .gameObject.SetActive(true );
			inputFoodInfo .gameObject.SetActive(false);
			DateTime date;
			try {
				date = new DateTime(
					int.Parse(inputFoodYear .text),
					int.Parse(inputFoodMonth.text),
					int.Parse(inputFoodDay  .text));
			}
			catch {
				date = DateTime.Now + new TimeSpan(7, 0, 0, 0);
			}
			FoodObject foodObject = AddFood(inputFoodName.text, date);
			foodList.Add(foodObject);
			RefreshFood();
		});
	}

	FoodObject AddFood(string name, DateTime date) {
		FoodObject foodObject = new FoodObject {
			gameObject = Instantiate(prefabFood, transform), name = name, expirationDate = date
		};
		foodObject.gameObject.name = foodObject.name;
		foodObject.gameObject.transform.GetChild(0).TryGetComponent(out text);
		text.text = foodObject.name;
		foodObject.gameObject.transform.GetChild(1).TryGetComponent(out text);
		text.text = foodObject.expirationDate.ToString("yyyy.MM.dd");
		foodObject.gameObject.transform.GetChild(2).TryGetComponent(out button);
		button.onClick.AddListener(() => {
			Destroy(foodObject.gameObject);
			foodList.Remove(foodObject);
			RefreshFood();
		});
		return foodObject;
	}

	void RefreshFood() {
		RectTransform contentFood;
		RectTransform contentFoodDangerous;
		rectFood         .transform.GetChild(1).GetChild(0).TryGetComponent(out contentFood);
		rectFoodDangerous.transform.GetChild(1).GetChild(0).TryGetComponent(out contentFoodDangerous);
		DateTime date = DateTime.Now + new TimeSpan(3, 0, 0, 0);

		int j = 0;
		int k = 0;
		for (int i = 0; i < foodList.Count; i++) {
			foodList[i].gameObject.TryGetComponent(out rect);
			if (date <= foodList[i].expirationDate) {
				foodList[i].gameObject.transform.SetParent(contentFood);
				rect.offsetMin = new Vector2(0,   0);
				rect.offsetMax = new Vector2(0, 100);
				rect.anchoredPosition = new Vector2(0, -j * 100);
				j++;
			}
			else {
				foodList[i].gameObject.transform.SetParent(contentFoodDangerous);
				rect.offsetMin = new Vector2(0,   0);
				rect.offsetMax = new Vector2(0, 100);
				rect.anchoredPosition = new Vector2(0, -k * 100);
				k++;
			}
			PlayerPrefs.SetString("name" + i, foodList[i].name);
			PlayerPrefs.SetString("date" + i, foodList[i].expirationDate.ToString("yyyy.MM.dd"));
		}
		PlayerPrefs.SetInt("foodCount", foodList.Count);

		contentFood         .sizeDelta = new Vector2(0, j * 100);
		contentFoodDangerous.sizeDelta = new Vector2(0, k * 100);

		if (j == 0)  rectFood.gameObject.SetActive(false);
		else         rectFood.gameObject.SetActive(true );
		if (k == 0) {
			rectFoodDangerous.gameObject.SetActive(false);
			rectFood.TryGetComponent(out rect);
			rect.anchoredPosition = new Vector2(0,  -820);
		}
		else {
			rectFoodDangerous.gameObject.SetActive(true );
			rectFood.TryGetComponent(out rect);
			rect.anchoredPosition = new Vector2(0, -1220);
		}
	}





	[Header("Recipe")]
	[SerializeField] Canvas pageRecipe;

	[SerializeField] InputField inputRecipe;
	[SerializeField] Button buttonSearch;

	[SerializeField] Button buttonKorean;
	[SerializeField] Button buttonChinese;
	[SerializeField] Button buttonJapanese;
	[SerializeField] Button buttonWestern;

	[SerializeField] ScrollRect rectRecipe;
	[SerializeField] RectTransform rectRecipePopup;
	[SerializeField] Text textRecipeName;
	[SerializeField] Text textRecipeFood;
	[SerializeField] Text textRecipeGram;
	[SerializeField] Text textRecipeBool;

	[SerializeField] Button buttonRecipeToAnalysis;

	[SerializeField] GameObject prefabRecipe;

	struct RecipeObject {
		public GameObject gameObject;
		public Database.Recipe recipe;
	}
	List<RecipeObject> recipeList = new List<RecipeObject>();

	void InitRecipe() {
		pageRecipe.gameObject.SetActive(false);
		rectRecipePopup.gameObject.SetActive(false);
		RefreshRecipe(Database.RecipeType.Korean);

		buttonSearch.onClick.AddListener(() => {
			bool exist = false;
			for (int i = 0; i < Database.recipe.Count; i++) {
				if (Database.recipe[i].name == inputRecipe.text) {
					PopupRecipe(Database.recipe[i]);
					break;
				}
			}
			if (!exist) ;
		});
		buttonKorean.onClick.AddListener(() => {
			RefreshRecipe(Database.RecipeType.Korean);
		});
		buttonChinese.onClick.AddListener(() => {
			RefreshRecipe(Database.RecipeType.Chinese);
		});
		buttonJapanese.onClick.AddListener(() => {
			RefreshRecipe(Database.RecipeType.Japanese);
		});
		buttonWestern.onClick.AddListener(() => {
			RefreshRecipe(Database.RecipeType.Western);
		});
		buttonRecipeToAnalysis.onClick.AddListener(() => {
			ChangePageTo(pageAnalysis);
			RefreshAnalysis();
		});
	}

	RecipeObject AddRecipe(Database.Recipe recipe, RectTransform parent = null) {
		RecipeObject recipeObject = new RecipeObject {
			gameObject = Instantiate(prefabRecipe, parent), recipe = recipe
		};
		recipeObject.gameObject.name = recipeObject.recipe.name;
		recipeObject.gameObject.transform.GetChild(0).TryGetComponent(out text);
		text.text = recipeObject.recipe.name;
		recipeObject.gameObject.transform.TryGetComponent(out button);
		button.onClick.AddListener(() => {
			PopupRecipe(recipeObject.recipe);
		});
		return recipeObject;
	}

	void RefreshRecipe(Database.RecipeType type) {
		RectTransform contentRecipe;
		rectRecipe.transform.GetChild(1).GetChild(0).TryGetComponent(out contentRecipe);

		for (int i = recipeList.Count - 1; -1 < i; i--) {
			Destroy(recipeList[i].gameObject);
			recipeList.RemoveAt(i);
		}
		int j = 0;
		for (int i = 0; i < Database.recipe.Count; i++) {
			if (Database.recipe[i].type == type) {
				RecipeObject recipeObject = AddRecipe(Database.recipe[i], contentRecipe);
				recipeList.Add(recipeObject);
				recipeObject.gameObject.TryGetComponent(out rect);
				rect.anchoredPosition = new Vector2(0, -j * 160);
				j++;
			}
		}
		contentRecipe.sizeDelta = new Vector2(0, j * 160);
	}

	void PopupRecipe(Database.Recipe recipe) {
		rectRecipePopup.gameObject.SetActive(true);
		textRecipeName.text = recipe.name;
		textRecipeFood.text = "";
		textRecipeGram.text = "";
		textRecipeBool.text = "";
		for (int i = 0; i < recipe.foodset.Count; i++) {
			bool exist = false;
			for (int j = 0; j < foodList.Count; j++) {
				if (recipe.foodset[i].food.name == foodList[j].name) {
					exist = true;
					break;
				}
			}
			string unit = Database.ToString(recipe.foodset[i].food.unit);
			textRecipeFood.text += recipe.foodset[i].food.name + "\n";
			textRecipeGram.text += recipe.foodset[i].amount + unit + "\n";
			textRecipeBool.text += exist ? "V \n" : "\n";
		}
		selected = recipe;
	}





	[Header("Analysis")]
	[SerializeField] Canvas pageAnalysis;

	[SerializeField] Text textSelected;
	[SerializeField] Text textSelectedFood;
	[SerializeField] Text textSelectedRestaurant;
	[SerializeField] RawImage imageRestaurant;
	[SerializeField] Button buttonOpenURL;

	Database.Recipe selected;
	Database.Restaurant selectedRestaurant;

	void InitAnalysis() {
		pageAnalysis.gameObject.SetActive(false);
		buttonOpenURL.onClick.AddListener(() => {
			Application.OpenURL(selectedRestaurant.url);
		});
	}

	void RefreshAnalysis() {
		textSelected.text = selected.name + " 만드는 데 드는 비용";
		textSelectedFood.text = "";

		bool exist = false;
		float total = 0.0f;
		for (int i = 0; i < selected.foodset.Count; i++) {
			exist = false;
			for (int j = 0; j < foodList.Count; j++) {
				if (selected.foodset[i].food.name == foodList[j].name) {
					exist = true;
					break;
				}
			}
			if (exist) continue;
			float amount = selected.foodset[i].amount;
			float price  = selected.foodset[i].food.price / selected.foodset[i].food.amount * amount;
			string unit = Database.ToString(selected.foodset[i].food.unit);
			textSelectedFood.text += selected.foodset[i].food.name + " ";
			textSelectedFood.text += amount.ToString("F0") + unit + " : ";
			textSelectedFood.text += price.ToString("F0") + "원\n";
			total += price;
		}
		textSelectedFood.text += "합계 : " + total + "원";

		exist = false;
		textSelectedRestaurant.text = "";
		for (int i = 0; i < Database.restaurant.Count; i++) {
			for (int j = 0; j < Database.restaurant[i].menuset.Count; j++) {
				if (Database.restaurant[i].menuset[j].recipe.name.Contains(selected.name)) {
					selectedRestaurant = Database.restaurant[i];
					textSelectedRestaurant.text += Database.restaurant[i].name + " : ";
					textSelectedRestaurant.text += Database.restaurant[i].menuset[j].price + "원\n";
					exist = true;
				}
			}
		}
		if (!exist) textSelectedRestaurant.text = "주변에 이 음식을 파는 식당이 없습니다.";
		buttonOpenURL.gameObject.SetActive(exist);
	}





	[Header("Status Bar")]
	[SerializeField] Canvas barStatus;
	[SerializeField] Text textTime;

	void InitStatusBar() {
		#if UNITY_EDITOR
			barStatus.gameObject.SetActive(true );
		#else
			barStatus.gameObject.SetActive(false);
		#endif
	}





	[Header("Navigation Bar")]
	[SerializeField] Canvas barNavigation;
	[SerializeField] Button buttonMenu;
	[SerializeField] Button buttonHome;
	[SerializeField] Button buttonBack;

	void InitNavigationBar() {
		#if UNITY_EDITOR
			barNavigation.gameObject.SetActive(true );
		#else
			barNavigation.gameObject.SetActive(false);
		#endif
		buttonBack.onClick.AddListener(() => { Back(); });
	}
}
