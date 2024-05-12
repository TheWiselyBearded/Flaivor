using Newtonsoft.Json.Linq;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Images;
using OpenAI.Models;
using OVR.OpenVR;
using OVRSimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Utilities.WebRequestRest;

public class ThinkerModule : MonoBehaviour
{
    public RawImage inputImage;
    private static bool isChatPending;
    private int attemptCount = 0;

    private OpenAIClient api;
    public bool preprompt;
    public PersonalityType Personality;
    private readonly List<Message> chatMessages = new List<Message>();

    public event Action<string, Texture> OnFullRecipeBookReceived;
    public event Action<string> OnChatGPTInputReceived;
    public event Action<string> OnChatGPTHelpInputReceived;


    private void Start()
    {
        api = new OpenAIClient();
    }

    /// <summary>
    /// entry point from other script to submit api request
    /// </summary>
    /// <param name="input"></param>
    public void GenerateResponse(string input) => SubmitChat(input);

    public string PrepromptQueryText(string text)
    {
        string complete = String.Empty;
        string preprompt = "Briefly and uniquely, as shortly as possible, please use no more than 4 sentences to respond to the following statement/question: ";
        complete = preprompt + text;
        return complete;
    }

    public async void SubmitChat(string userInput)
    {
        if (isChatPending || string.IsNullOrWhiteSpace(userInput)) { return; }
        isChatPending = true;

        if (preprompt) userInput = PrepromptQueryText(userInput);
        var userMessage = new Message(Role.User, userInput);
        chatMessages.Add(userMessage);

        try
        {
            var chatRequest = new ChatRequest(chatMessages, Model.GPT3_5_Turbo_16K);
            var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
            var response = result.ToString();

            Debug.Log(response);
            OnChatGPTInputReceived?.Invoke(response);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            //if (lifetimeCancellationTokenSource != null) {}
            isChatPending = false;
        }
    }

    /// <summary>
    /// invoked when voice input is given of ingredients
    /// system command: I will give you a photo or list of ingredients and you will ALWAYS generate 3 recipes based on the given ingredients. For each recipe please include a Title, a short description of the dish, list of all ingredients, and a detailed list of instructions for making the dish. The instructions for the dish should be separated into numbered sections with smaller detailed steps for the section underneath. All of this should be formatted as a JSON file. make sure the code is complete and ALWAYS includes 3 recipes. Only respond with the json always and ALWAYS ALWAYS INCLUDE 3 RECIPES.
    /// // example query: I have the following ingredients with me, Eggs, spices, green onions, steak, potatoes, tortilla, bread, sugar, and milk
    /// </summary>
    /// <param name="userInput"></param>
    public async void SubmitChatJSON(string userInput)
    {
        if (isChatPending || string.IsNullOrWhiteSpace(userInput)) { return; }
        isChatPending = true;

        var messages = new List<Message>
        {
            new Message(Role.System, "I will give you a photo or list of ingredients and you will ALWAYS generate 3 recipes based on the given ingredients. For each recipe please include a Title, a short description of the dish, list of all ingredients, and a detailed list of instructions for making the dish. The instructions for the dish should be separated into numbered sections with smaller detailed steps for the section underneath. All of this should be formatted as a JSON file. make sure the code is complete and ALWAYS includes 3 recipes. Only respond with the json always and ALWAYS ALWAYS INCLUDE 3 RECIPES. \\n\\n\\nExample output:\\n[\\n  {\\n    \\\"recipe_name\\\": \\\"Beef Stroganoff\\\",\\n    \\\"description\\\": \\\"Beef Stroganoff is a comforting Russian dish featuring tender strips of beef cooked with mushrooms and onions in a creamy sauce flavored with sour cream, Dijon mustard, and Worcestershire sauce. Served over egg noodles, it's a satisfying meal perfect for cozy evenings.\\\",\\n    \\\"ingredients\\\": {\\n      \\\"beef_strips\\\": \\\"1 pound sirloin steak, cut into thin strips\\\",\\n      \\\"salt\\\": \\\"1 teaspoon\\\",\\n      \\\"black_pepper\\\": \\\"1/2 teaspoon\\\",\\n      \\\"butter\\\": \\\"2 tablespoons\\\",\\n      \\\"olive_oil\\\": \\\"1 tablespoon\\\",\\n      \\\"onion\\\": \\\"1 large, thinly sliced\\\",\\n      \\\"mushrooms\\\": \\\"8 ounces, sliced\\\",\\n      \\\"garlic_cloves\\\": \\\"2, minced\\\",\\n      \\\"all-purpose_flour\\\": \\\"2 tablespoons\\\",\\n      \\\"beef_broth\\\": \\\"1 cup\\\",\\n      \\\"sour_cream\\\": \\\"1/2 cup\\\",\\n      \\\"Dijon_mustard\\\": \\\"1 tablespoon\\\",\\n      \\\"Worcestershire_sauce\\\": \\\"1 tablespoon\\\",\\n      \\\"egg_noodles\\\": \\\"12 ounces, cooked according to package instructions\\\",\\n      \\\"chopped_parsley\\\": \\\"for garnish\\\"\\n    },\\n    \\\"instructions\\\": [\\n      {\\n        \\\"step_number\\\": 1,\\n        \\\"description\\\": \\\"Cook the Beef:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Season the beef strips with salt and black pepper.\\\",\\n          \\\"In a large skillet, melt 1 tablespoon of butter with 1 tablespoon of olive oil over medium-high heat.\\\",\\n          \\\"Add the beef strips and cook until browned, about 3-4 minutes. Transfer beef to a plate and set aside.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 2,\\n        \\\"description\\\": \\\"Saute the Onion and Mushrooms:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"In the same skillet, melt the remaining tablespoon of butter.\\\",\\n          \\\"Add the sliced onion and cook until softened, about 3 minutes.\\\",\\n          \\\"Add the sliced mushrooms and cook until they release their juices and become tender, about 5 minutes.\\\",\\n          \\\"Stir in the minced garlic and cook for an additional minute.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 3,\\n        \\\"description\\\": \\\"Make the Sauce:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Sprinkle the cooked onion and mushrooms with flour and stir to combine.\\\",\\n          \\\"Gradually pour in the beef broth, stirring constantly to prevent lumps from forming.\\\",\\n          \\\"Bring the mixture to a simmer and cook until the sauce thickens, about 5 minutes.\\\",\\n          \\\"Reduce the heat to low and stir in the sour cream, Dijon mustard, and Worcestershire sauce until well combined.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 4,\\n        \\\"description\\\": \\\"Combine and Serve:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Return the cooked beef strips to the skillet and stir until heated through.\\\",\\n          \\\"Serve the beef stroganoff over cooked egg noodles, garnished with chopped parsley.\\\",\\n          \\\"Enjoy your delicious beef stroganoff!\\\"\\n        ]\\n      }\\n    ]\\n  },\\n  {\\n    \\\"recipe_name\\\": \\\"Grilled Cheese\\\",\\n    \\\"description\\\": \\\"Grilled Cheese is a classic comfort food sandwich made with buttered bread slices and melted cheese. It's simple yet delicious, with the option to add extras like sliced tomato, cooked bacon, ham, or avocado for a personalized touch. Perfectly crispy on the outside and gooey on the inside, it's a timeless favorite enjoyed by all ages.\\\",\\n    \\\"ingredients\\\": {\\n      \\\"bread_slices\\\": \\\"4 slices\\\",\\n      \\\"butter\\\": \\\"2 tablespoons, softened\\\",\\n      \\\"cheese_slices\\\": \\\"4 slices (cheddar, American, or your favorite melting cheese)\\\",\\n      \\\"optional_additions\\\": \\\"sliced tomato, cooked bacon, ham, or avocado (optional)\\\"\\n    },\\n    \\\"instructions\\\": [\\n      {\\n        \\\"step_number\\\": 1,\\n        \\\"description\\\": \\\"Assemble the Sandwiches:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Spread one side of each bread slice with softened butter.\\\",\\n          \\\"Place one slice of cheese on the unbuttered side of 2 bread slices.\\\",\\n          \\\"If using any optional additions like sliced tomato, cooked bacon, ham, or avocado, layer them on top of the cheese slices.\\\",\\n          \\\"Top each sandwich with the remaining bread slices, buttered side facing out.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 2,\\n        \\\"description\\\": \\\"Cook the Sandwiches:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Heat a skillet or griddle over medium heat.\\\",\\n          \\\"Place the assembled sandwiches in the skillet and cook until golden brown and crispy on one side, about 2-3 minutes.\\\",\\n          \\\"Flip the sandwiches and cook until the other side is golden brown and the cheese is melted, about 2-3 minutes more.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 3,\\n        \\\"description\\\": \\\"Serve:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Remove the grilled cheese sandwiches from the skillet and let them cool for a minute.\\\",\\n          \\\"Cut the sandwiches in half diagonally, if desired, and serve hot.\\\",\\n          \\\"Enjoy your delicious grilled cheese sandwiches!\\\"\\n        ]\\n      }\\n    ]\\n  },\\n  {\\n    \\\"recipe_name\\\": \\\"Spaghetti and Meatballs\\\",\\n    \\\"description\\\": \\\"Spaghetti and Meatballs is an iconic Italian-American dish featuring tender meatballs simmered in a rich tomato sauce, served over a bed of al dente spaghetti noodles. With flavors of garlic, Parmesan cheese, and fresh herbs, it's a hearty and satisfying meal loved by many.\\\",\\n    \\\"ingredients\\\": {\\n      \\\"ground_beef\\\": \\\"1 pound\\\",\\n      \\\"breadcrumbs\\\": \\\"1/2 cup\\\",\\n      \\\"grated_Parmesan_cheese\\\": \\\"1/4 cup\\\",\\n      \\\"fresh_parsley\\\": \\\"1/4 cup\\\",\\n      \\\"garlic_cloves\\\": \\\"6\\\",\\n      \\\"large_egg\\\": \\\"1\\\",\\n      \\\"salt\\\": \\\"1 teaspoon\\\",\\n      \\\"black_pepper\\\": \\\"1/2 teaspoon\\\",\\n      \\\"dried_oregano\\\": \\\"1 1/4 teaspoons\\\",\\n      \\\"dried_basil\\\": \\\"3/4 teaspoon\\\",\\n      \\\"olive_oil\\\": \\\"1 tablespoon + for frying\\\",\\n      \\\"small_onion\\\": \\\"1\\\",\\n      \\\"crushed_tomatoes\\\": \\\"1 (28-ounce) can\\\",\\n      \\\"sugar\\\": \\\"pinch (optional)\\\",\\n      \\\"spaghetti\\\": \\\"12 ounces\\\",\\n      \\\"grated_Parmesan_cheese_garnish\\\": \\\"for garnish\\\",\\n      \\\"chopped_fresh_basil_or_parsley_garnish\\\": \\\"for garnish\\\"\\n    },\\n    \\\"instructions\\\": [\\n      {\\n        \\\"step_number\\\": 1,\\n        \\\"description\\\": \\\"Prepare the Meatballs:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"In a large mixing bowl, combine the ground beef, breadcrumbs, grated Parmesan cheese, minced parsley, minced garlic, egg, salt, black pepper, dried oregano, and dried basil. Mix until well combined.\\\",\\n          \\\"Shape the mixture into meatballs, about 1 to 1.5 inches in diameter.\\\",\\n          \\\"Heat olive oil in a large skillet over medium heat. Cook the meatballs in batches until browned on all sides and cooked through, about 8-10 minutes. Transfer cooked meatballs to a plate and set aside.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 2,\\n        \\\"description\\\": \\\"Make the Tomato Sauce:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"In the same skillet used for cooking the meatballs, add another tablespoon of olive oil if needed. Add the chopped onion and cook until softened, about 3-4 minutes.\\\",\\n          \\\"Stir in the minced garlic and cook for an additional minute until fragrant.\\\",\\n          \\\"Pour in the crushed tomatoes, dried oregano, dried basil, salt, black pepper, and a pinch of sugar if desired. Bring the sauce to a simmer, then reduce the heat to low. Let it simmer for about 15-20 minutes, stirring occasionally.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 3,\\n        \\\"description\\\": \\\"Cook the Spaghetti:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"While the sauce is simmering, cook the spaghetti according to the package instructions until al dente. Once cooked, drain the spaghetti and return it to the pot.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 4,\\n        \\\"description\\\": \\\"Combine and Serve:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Once the sauce has simmered and the meatballs are cooked, add the meatballs to the sauce and let them simmer together for an additional 5 minutes to heat through.\\\",\\n          \\\"Serve the spaghetti topped with the meatballs and sauce. Garnish with grated Parmesan cheese and chopped fresh basil or parsley, if desired.\\\",\\n          \\\"Enjoy your delicious spaghetti and meatballs!\\\"\\n        ]\\n      }\\n    ]\\n  }\\n]\\n\"\r\n    },\r\n    {\r\n      \"role\": \"user\",\r\n      \"content\": \"Eggs, spices, green onions, steak, potatoes, tortilla, bread, sugar, and milk \"\r\n    },\r\n    {\r\n      \"role\": \"assistant\",\r\n      \"content\": \"[\\n  {\\n    \\\"recipe_name\\\": \\\"Steak and Eggs Breakfast Burrito\\\",\\n    \\\"description\\\": \\\"This hearty breakfast burrito is filled with tender steak, scrambled eggs, and sautéed vegetables, all wrapped in a warm tortilla. It's a filling and flavorful way to start your day!\\\",\\n    \\\"ingredients\\\": {\\n      \\\"steak\\\": \\\"8 ounces, thinly sliced\\\",\\n      \\\"eggs\\\": \\\"4\\\",\\n      \\\"olive_oil\\\": \\\"1 tablespoon\\\",\\n      \\\"green_onions\\\": \\\"3, chopped\\\",\\n      \\\"salt\\\": \\\"1/2 teaspoon\\\",\\n      \\\"black_pepper\\\": \\\"1/4 teaspoon\\\",\\n      \\\"tortilla\\\": \\\"4 large\\\",\\n      \\\"optional_toppings\\\": \\\"salsa, sour cream, shredded cheese\\\"\\n    },\\n    \\\"instructions\\\": [\\n      {\\n        \\\"step_number\\\": 1,\\n        \\\"description\\\": \\\"Cook the Steak:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Season the steak with salt and black pepper.\\\",\\n          \\\"In a large skillet, heat olive oil over medium-high heat.\\\",\\n          \\\"Add the steak slices to the skillet and cook until browned and cooked to your desired level of doneness, about 2-3 minutes per side. Transfer the cooked steak to a plate and set aside.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 2,\\n        \\\"description\\\": \\\"Scramble the Eggs:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"In the same skillet, add the chopped green onions and sauté for 2 minutes until softened.\\\",\\n          \\\"In a bowl, beat the eggs with salt and black pepper.\\\",\\n          \\\"Pour the beaten eggs into the skillet with the green onions and cook, stirring constantly, until the eggs are scrambled and cooked to your desired consistency.\\\",\\n          \\\"Remove the skillet from heat.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 3,\\n        \\\"description\\\": \\\"Assemble the Burritos:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Warm the tortillas in a dry skillet or microwave until pliable.\\\",\\n          \\\"Place a portion of the scrambled eggs in the center of each tortilla.\\\",\\n          \\\"Top the eggs with a few slices of cooked steak.\\\",\\n          \\\"Add any optional toppings such as salsa, sour cream, or shredded cheese.\\\",\\n          \\\"Roll up the tortillas, folding in the sides as you go, to form burritos.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 4,\\n        \\\"description\\\": \\\"Serve:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Slice each burrito in half diagonally, if desired, and serve hot.\\\",\\n          \\\"Enjoy your delicious steak and eggs breakfast burrito!\\\"\\n        ]\\n      }\\n    ]\\n  },\\n  {\\n    \\\"recipe_name\\\": \\\"Spicy Roasted Potatoes\\\",\\n    \\\"description\\\": \\\"These spicy roasted potatoes are crispy on the outside, tender on the inside, and packed with flavor. The combination of spices adds a kick to the potatoes, making them a delicious side dish or snack.\\\",\\n    \\\"ingredients\\\": {\\n      \\\"potatoes\\\": \\\"2 pounds, cut into small cubes\\\",\\n      \\\"olive_oil\\\": \\\"2 tablespoons\\\",\\n      \\\"paprika\\\": \\\"1 teaspoon\\\",\\n      \\\"garlic_powder\\\": \\\"1 teaspoon\\\",\\n      \\\"onion_powder\\\": \\\"1/2 teaspoon\\\",\\n      \\\"cayenne_pepper\\\": \\\"1/2 teaspoon\\\",\\n      \\\"salt\\\": \\\"1/2 teaspoon\\\",\\n      \\\"black_pepper\\\": \\\"1/4 teaspoon\\\",\\n      \\\"optional_garnish\\\": \\\"chopped parsley\\\"\\n    },\\n    \\\"instructions\\\": [\\n      {\\n        \\\"step_number\\\": 1,\\n        \\\"description\\\": \\\"Preheat the Oven:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Preheat the oven to 425°F (220°C).\\\",\\n          \\\"Line a baking sheet with parchment paper or foil for easy cleanup.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 2,\\n        \\\"description\\\": \\\"Coat the Potatoes:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"In a large mixing bowl, combine the cubed potatoes, olive oil, paprika, garlic powder, onion powder, cayenne pepper, salt, and black pepper.\\\",\\n          \\\"Toss the potatoes until they are evenly coated with the spice mixture.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 3,\\n        \\\"description\\\": \\\"Roast the Potatoes:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Spread the coated potatoes in a single layer on the prepared baking sheet.\\\",\\n          \\\"Place the baking sheet in the preheated oven and roast for 25-30 minutes, or until the potatoes are golden brown and crispy on the outside, and tender on the inside.\\\",\\n          \\\"Flip the potatoes halfway through cooking to ensure even browning.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 4,\\n        \\\"description\\\": \\\"Garnish and Serve:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Remove the roasted potatoes from the oven and let them cool for a few minutes.\\\",\\n          \\\"Sprinkle with chopped parsley, if desired, for added freshness and color.\\\",\\n          \\\"Serve hot as a side dish or snack.\\\",\\n          \\\"Enjoy your spicy roasted potatoes!\\\"\\n        ]\\n      }\\n    ]\\n  },\\n  {\\n    \\\"recipe_name\\\": \\\"French Toast\\\",\\n    \\\"description\\\": \\\"This classic French toast recipe turns ordinary bread into a decadent and satisfying breakfast or brunch dish. The bread slices are soaked in a sweetened egg and milk mixture, then cooked until golden brown. Serve with maple syrup, powdered sugar, or fresh fruits for a delightful morning treat.\\\",\\n    \\\"ingredients\\\": {\\n      \\\"bread_slices\\\": \\\"6\\\",\\n      \\\"eggs\\\": \\\"3\\\",\\n      \\\"milk\\\": \\\"3/4 cup\\\",\\n      \\\"sugar\\\": \\\"2 tablespoons\\\",\\n      \\\"cinnamon\\\": \\\"1/2 teaspoon\\\",\\n      \\\"vanilla_extract\\\": \\\"1/2 teaspoon\\\",\\n      \\\"butter\\\": \\\"2 tablespoons\\\",\\n      \\\"optional_toppings\\\": \\\"maple syrup, powdered sugar, fresh fruits\\\"\\n    },\\n    \\\"instructions\\\": [\\n      {\\n        \\\"step_number\\\": 1,\\n        \\\"description\\\": \\\"Prepare the Egg Mixture:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"In a shallow bowl or pie dish, whisk together the eggs, milk, sugar, cinnamon, and vanilla extract until well combined.\\\",\\n          \\\"Place the bread slices in a single layer in a separate baking dish or on a large plate.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 2,\\n        \\\"description\\\": \\\"Soak the Bread Slices:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Pour the egg mixture over the bread slices, ensuring that each slice is fully coated.\\\",\\n          \\\"Allow the bread to soak in the mixture for a few seconds on each side, until it becomes slightly saturated.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 3,\\n        \\\"description\\\": \\\"Cook the French Toast:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"In a large skillet or griddle, melt the butter over medium heat.\\\",\\n          \\\"Place the soaked bread slices in the skillet and cook until golden brown, about 2-3 minutes per side.\\\",\\n          \\\"If necessary, work in batches to avoid overcrowding the skillet.\\\",\\n          \\\"Remove the cooked French toast from the skillet and keep it warm while cooking the remaining slices.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 4,\\n        \\\"description\\\": \\\"Serve:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Transfer the cooked French toast to serving plates.\\\",\\n          \\\"Drizzle with maple syrup, sprinkle with powdered sugar, and garnish with fresh fruits, if desired.\\\",\\n          \\\"Serve immediately and enjoy your delicious homemade French toast!\\\"\\n        ]\\n      }\\n    ]\\n  }\\n],\n...\n]"),
            new Message(Role.User, userInput),
        };

        foreach(var message in messages) chatMessages.Add(message);
        Debug.Log("Added all messages");

        try
        {
            //var chatRequest = new ChatRequest(chatMessages, Model.GPT4_Turbo, responseFormat: ChatResponseFormat.Json, temperature: 1, maxTokens: 14421, topP: 1, frequencyPenalty: 0, presencePenalty: 0);
            var chatRequest = new ChatRequest(chatMessages, Model.GPT3_5_Turbo_16K, temperature: 1, topP: 1, frequencyPenalty: 0, presencePenalty: 0);
            var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
            
            var response = RemoveEmbeddedCharacters(result.ToString());
            string formattedResponse = CookSessionController.EnsureJsonWrappedWithRecipesKey(response);
            Debug.Log(formattedResponse);
            OnChatGPTInputReceived?.Invoke(formattedResponse);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            //if (lifetimeCancellationTokenSource != null) {}
            isChatPending = false;
        }
    }

    public async Task<Texture> SubmitChatImageGenerator(string userInput) {
        if (isChatPending || string.IsNullOrWhiteSpace(userInput)) { return null; }
        isChatPending = true;

        var msg = "I will give you example prompts of how to generatge an image titled `EXAMPLE`. Then I will give you the actual prompt, titled `ACTUAL`:";
        msg += "\nEXAMPLE: Role.System: I'll give you a dish name and description and you generate a prompt to produce a detailed photo-realistic version of the dish in DALL-E. The photo must have the food on a white plate with a blank white background at a 45-degree angle facing the camera. Nothing else should be in the scene.";
        msg += "\nEXAMPLE: Role.User: French Toast\n Description: Indulge in a classic breakfast favorite with this easy French toast recipe. Thick slices of bread are soaked in a sweet and creamy mixture of eggs, milk, and sugar, then cooked to golden perfection. Serve with maple syrup or your favorite toppings for a delectable morning ";
        msg += "\nEXAMPLE: Role.Assistant: Create a detailed photo-realistic version of French Toast on a white plate with a blank white background at a 45-degree angle facing the camera. The French Toast should be thick slices of bread soaked in a sweet and creamy mixture of eggs, milk, and sugar, cooked to golden perfection. Include a side of maple syrup or other toppings to accompany the dish.";
        msg += $"\nACTUAL: Role.User + {userInput}";
        //foreach (var message in messages) chatMessages.Add(message);
        Debug.Log("Sending image generator request");

        try {
            var request = new ImageGenerationRequest(msg, Model.DallE_3);
            var imageResults = await api.ImagesEndPoint.GenerateImageAsync(request);
            Debug.Log("OBtained results");
            foreach (var result in imageResults) {
                Debug.Log(result.ToString());
                Assert.IsNotNull(result.Texture);
            }
            return imageResults[0].Texture;
            //OnFullRecipeBookReceived?.Invoke();
        } catch (Exception e) {
            Debug.LogError(e);
            return null;
        } finally {
            //if (lifetimeCancellationTokenSource != null) {}
            isChatPending = false;
        }
    }

    public async void SubmitChatHelpJSON(string userInput) {
        if (isChatPending || string.IsNullOrWhiteSpace(userInput)) { return; }
        isChatPending = true;

        var messages = new List<Message>
        {
            new Message(Role.System, "I will ask you for help regarding something I'm cooking. Please swiftly and concisely answer inquiries related to cooking."),
            new Message(Role.User, userInput),
        };

        foreach (var message in messages) chatMessages.Add(message);
        Debug.Log("Added all messages");

        try {
            //var chatRequest = new ChatRequest(chatMessages, Model.GPT4_Turbo, responseFormat: ChatResponseFormat.Json, temperature: 1, maxTokens: 14421, topP: 1, frequencyPenalty: 0, presencePenalty: 0);
            var chatRequest = new ChatRequest(chatMessages, Model.GPT3_5_Turbo, temperature: 1, topP: 1, frequencyPenalty: 0, presencePenalty: 0);
            var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);

            var response = result.ToString();
            Debug.Log($"Help request response {response}");
            OnChatGPTHelpInputReceived?.Invoke(response);
        } catch (Exception e) {
            Debug.LogError(e);
        } finally {
            //if (lifetimeCancellationTokenSource != null) {}
            isChatPending = false;
        }
    }

    private string EnsureThreeRecipes(string jsonResponse)
    {
        try
        {
            var jObject = JObject.Parse(jsonResponse);
            var recipes = jObject["recipes"];

            if (recipes == null || recipes.Count() < 3)
            {
                // Check if there's exactly one recipe object not wrapped in an array
                if (jObject["recipe_name"] != null)
                {
                    // It's a single recipe object, wrap it in an array
                    jObject["recipes"] = new JArray(jObject);
                    jObject.Remove("recipe_name");
                    jObject.Remove("ingredients");
                    jObject.Remove("instructions");
                }
                Debug.Log("Adjusted JSON to contain an array of recipes.");
            }

            return jObject.ToString();
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to parse or adjust JSON response: " + e.Message);
            return jsonResponse; // Return the original if parsing fails
        }
    }

    public async void SubmitChatJSONRecursive(string userInput)
    {
        if (isChatPending || string.IsNullOrWhiteSpace(userInput)) { return; }
        isChatPending = true;

        if (attemptCount >= 3)  // Check if maximum attempts have been reached
        {
            Debug.LogError("Maximum attempt limit reached without sufficient recipes.");
            attemptCount = 0;  // Reset the attempt counter for future calls
            isChatPending = false;
            return;  // Exit the method
        }

        var messages = new List<Message>
        {
            new Message(Role.System, "I will give you a photo or list of ingredients and you will ALWAYS generate 3 recipes based on the given ingredients. For each recipe please include a Title, a short description of the dish, list of all ingredients, and a detailed list of instructions for making the dish. The instructions for the dish should be separated into numbered sections with smaller detailed steps for the section underneath. All of this should be formatted as a JSON file. make sure the code is complete and includes 3 recipes.\n\nExample output:\n[\n{\n  \"recipe_name\": \"Spaghetti and Meatballs\",\n  \"ingredients\": {\n    \"ground_beef\": \"1 pound\",\n    \"breadcrumbs\": \"1/2 cup\",\n    \"grated_Parmesan_cheese\": \"1/4 cup\",\n    \"fresh_parsley\": \"1/4 cup\",\n    \"garlic_cloves\": \"6\",\n    \"large_egg\": \"1\",\n    \"salt\": \"1 teaspoon\",\n    \"black_pepper\": \"1/2 teaspoon\",\n    \"dried_oregano\": \"1 1/4 teaspoons\",\n    \"dried_basil\": \"3/4 teaspoon\",\n    \"olive_oil\": \"1 tablespoon + for frying\",\n    \"small_onion\": \"1\",\n    \"crushed_tomatoes\": \"1 (28-ounce) can\",\n    \"sugar\": \"pinch (optional)\",\n    \"spaghetti\": \"12 ounces\",\n    \"grated_Parmesan_cheese_garnish\": \"for garnish\",\n    \"chopped_fresh_basil_or_parsley_garnish\": \"for garnish\"\n  },\n  \"instructions\": [\n    {\n      \"step_number\": 1,\n      \"description\": \"Prepare the Meatballs:\",\n      \"sub_steps\": [\n        \"In a large mixing bowl, combine the ground beef, breadcrumbs, grated Parmesan cheese, minced parsley, minced garlic, egg, salt, black pepper, dried oregano, and dried basil. Mix until well combined.\",\n        \"Shape the mixture into meatballs, about 1 to 1.5 inches in diameter.\",\n        \"Heat olive oil in a large skillet over medium heat. Cook the meatballs in batches until browned on all sides and cooked through, about 8-10 minutes. Transfer cooked meatballs to a plate and set aside.\"\n      ]\n    },\n    ...\n  ]\n},\n...\n]"),
            new Message(Role.User, "Eggs, spices, green onions, steak, potatoes, tortilla, bread, sugar, and milk"),
        };

        foreach (var message in messages) chatMessages.Add(message);
        Debug.Log("Added all messages");

        try
        {
            //var chatRequest = new ChatRequest(chatMessages, Model.GPT4_Turbo, responseFormat: ChatResponseFormat.Json, temperature: 1, maxTokens: 14421, topP: 1, frequencyPenalty: 0, presencePenalty: 0);
            var chatRequest = new ChatRequest(chatMessages, Model.GPT4_Turbo, responseFormat: ChatResponseFormat.Json, temperature: 1, topP: 1, frequencyPenalty: 0, presencePenalty: 0);
            var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
            var response = result.ToString();

            Debug.Log("Checking response for three recipes...");
            if (!ContainsThreeRecipes(response))
            {
                Debug.Log("Not enough recipes, resubmitting...");
                attemptCount++;  // Increment the attempt counter
                this.SubmitChatJSON(userInput); // Resubmit if not enough recipes
            }
            else
            {
                Debug.Log("Sufficient recipes received.");
                OnChatGPTInputReceived?.Invoke(response);
                attemptCount = 0;  // Reset the attempt counter on success
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            isChatPending = false;
        }
    }

    private bool ContainsThreeRecipes(string jsonResponse)
    {
        try
        {
            var jObject = JObject.Parse(jsonResponse);
            var recipes = jObject["recipes"];
            return recipes != null && recipes.Count() == 3;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to parse JSON response: " + e.Message);
            return false;
        }
    }

    public void SubmitScreenshotChatRequest()
    {
        SubmitScreenshotChat();
    }

    public Texture2D ConvertRawImageToTexture2D()
    {
        if (inputImage == null || inputImage.texture == null)
        {
            Debug.LogError("RawImage or its texture is null.");
            return null;
        }

        Texture2D texture = new Texture2D(inputImage.texture.width, inputImage.texture.height);
        texture.SetPixels(((Texture2D)inputImage.texture).GetPixels());
        texture.Apply();
        return texture;
    }

    /// <summary>
    /// I will give you a photo or list of ingredients and you will ALWAYS generate 3 recipes based on the given ingredients. For each recipe please include a Title, a short description of the dish, list of all ingredients, and a detailed list of instructions for making the dish. The instructions for the dish should be separated into numbered sections with smaller detailed steps for the section underneath. All of this should be formatted as a JSON file. make sure the code is complete and ALWAYS includes 3 recipes. Only respond with the json always.\n\n
    /// </summary>
    public async void SubmitScreenshotChat()
    {
        if (api == null)
        {
            Debug.LogError("OpenAIClient is not initialized.");
            return;
        }

        var messages = new List<Message>
        {
            new Message(Role.System, "I will give you a photo or list of ingredients and you will ALWAYS generate 3 recipes based on the given ingredients. For each recipe please include a Title, a short description of the dish, list of all ingredients, and a detailed list of instructions for making the dish. The instructions for the dish should be separated into numbered sections with smaller detailed steps for the section underneath. All of this should be formatted as a JSON file. make sure the code is complete and ALWAYS includes 3 recipes. Only respond with the json always.\n\n\nExample output:\\n[\\n  {\\n    \\\"recipe_name\\\": \\\"Beef Stroganoff\\\",\\n    \\\"description\\\": \\\"Beef Stroganoff is a comforting Russian dish featuring tender strips of beef cooked with mushrooms and onions in a creamy sauce flavored with sour cream, Dijon mustard, and Worcestershire sauce. Served over egg noodles, it's a satisfying meal perfect for cozy evenings.\\\",\\n    \\\"ingredients\\\": {\\n      \\\"beef_strips\\\": \\\"1 pound sirloin steak, cut into thin strips\\\",\\n      \\\"salt\\\": \\\"1 teaspoon\\\",\\n      \\\"black_pepper\\\": \\\"1/2 teaspoon\\\",\\n      \\\"butter\\\": \\\"2 tablespoons\\\",\\n      \\\"olive_oil\\\": \\\"1 tablespoon\\\",\\n      \\\"onion\\\": \\\"1 large, thinly sliced\\\",\\n      \\\"mushrooms\\\": \\\"8 ounces, sliced\\\",\\n      \\\"garlic_cloves\\\": \\\"2, minced\\\",\\n      \\\"all-purpose_flour\\\": \\\"2 tablespoons\\\",\\n      \\\"beef_broth\\\": \\\"1 cup\\\",\\n      \\\"sour_cream\\\": \\\"1/2 cup\\\",\\n      \\\"Dijon_mustard\\\": \\\"1 tablespoon\\\",\\n      \\\"Worcestershire_sauce\\\": \\\"1 tablespoon\\\",\\n      \\\"egg_noodles\\\": \\\"12 ounces, cooked according to package instructions\\\",\\n      \\\"chopped_parsley\\\": \\\"for garnish\\\"\\n    },\\n    \\\"instructions\\\": [\\n      {\\n        \\\"step_number\\\": 1,\\n        \\\"description\\\": \\\"Cook the Beef:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Season the beef strips with salt and black pepper.\\\",\\n          \\\"In a large skillet, melt 1 tablespoon of butter with 1 tablespoon of olive oil over medium-high heat.\\\",\\n          \\\"Add the beef strips and cook until browned, about 3-4 minutes. Transfer beef to a plate and set aside.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 2,\\n        \\\"description\\\": \\\"Saute the Onion and Mushrooms:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"In the same skillet, melt the remaining tablespoon of butter.\\\",\\n          \\\"Add the sliced onion and cook until softened, about 3 minutes.\\\",\\n          \\\"Add the sliced mushrooms and cook until they release their juices and become tender, about 5 minutes.\\\",\\n          \\\"Stir in the minced garlic and cook for an additional minute.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 3,\\n        \\\"description\\\": \\\"Make the Sauce:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Sprinkle the cooked onion and mushrooms with flour and stir to combine.\\\",\\n          \\\"Gradually pour in the beef broth, stirring constantly to prevent lumps from forming.\\\",\\n          \\\"Bring the mixture to a simmer and cook until the sauce thickens, about 5 minutes.\\\",\\n          \\\"Reduce the heat to low and stir in the sour cream, Dijon mustard, and Worcestershire sauce until well combined.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 4,\\n        \\\"description\\\": \\\"Combine and Serve:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Return the cooked beef strips to the skillet and stir until heated through.\\\",\\n          \\\"Serve the beef stroganoff over cooked egg noodles, garnished with chopped parsley.\\\",\\n          \\\"Enjoy your delicious beef stroganoff!\\\"\\n        ]\\n      }\\n    ]\\n  },\\n  {\\n    \\\"recipe_name\\\": \\\"Grilled Cheese\\\",\\n    \\\"description\\\": \\\"Grilled Cheese is a classic comfort food sandwich made with buttered bread slices and melted cheese. It's simple yet delicious, with the option to add extras like sliced tomato, cooked bacon, ham, or avocado for a personalized touch. Perfectly crispy on the outside and gooey on the inside, it's a timeless favorite enjoyed by all ages.\\\",\\n    \\\"ingredients\\\": {\\n      \\\"bread_slices\\\": \\\"4 slices\\\",\\n      \\\"butter\\\": \\\"2 tablespoons, softened\\\",\\n      \\\"cheese_slices\\\": \\\"4 slices (cheddar, American, or your favorite melting cheese)\\\",\\n      \\\"optional_additions\\\": \\\"sliced tomato, cooked bacon, ham, or avocado (optional)\\\"\\n    },\\n    \\\"instructions\\\": [\\n      {\\n        \\\"step_number\\\": 1,\\n        \\\"description\\\": \\\"Assemble the Sandwiches:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Spread one side of each bread slice with softened butter.\\\",\\n          \\\"Place one slice of cheese on the unbuttered side of 2 bread slices.\\\",\\n          \\\"If using any optional additions like sliced tomato, cooked bacon, ham, or avocado, layer them on top of the cheese slices.\\\",\\n          \\\"Top each sandwich with the remaining bread slices, buttered side facing out.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 2,\\n        \\\"description\\\": \\\"Cook the Sandwiches:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Heat a skillet or griddle over medium heat.\\\",\\n          \\\"Place the assembled sandwiches in the skillet and cook until golden brown and crispy on one side, about 2-3 minutes.\\\",\\n          \\\"Flip the sandwiches and cook until the other side is golden brown and the cheese is melted, about 2-3 minutes more.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 3,\\n        \\\"description\\\": \\\"Serve:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Remove the grilled cheese sandwiches from the skillet and let them cool for a minute.\\\",\\n          \\\"Cut the sandwiches in half diagonally, if desired, and serve hot.\\\",\\n          \\\"Enjoy your delicious grilled cheese sandwiches!\\\"\\n        ]\\n      }\\n    ]\\n  },\\n  {\\n    \\\"recipe_name\\\": \\\"Spaghetti and Meatballs\\\",\\n    \\\"description\\\": \\\"Spaghetti and Meatballs is an iconic Italian-American dish featuring tender meatballs simmered in a rich tomato sauce, served over a bed of al dente spaghetti noodles. With flavors of garlic, Parmesan cheese, and fresh herbs, it's a hearty and satisfying meal loved by many.\\\",\\n    \\\"ingredients\\\": {\\n      \\\"ground_beef\\\": \\\"1 pound\\\",\\n      \\\"breadcrumbs\\\": \\\"1/2 cup\\\",\\n      \\\"grated_Parmesan_cheese\\\": \\\"1/4 cup\\\",\\n      \\\"fresh_parsley\\\": \\\"1/4 cup\\\",\\n      \\\"garlic_cloves\\\": \\\"6\\\",\\n      \\\"large_egg\\\": \\\"1\\\",\\n      \\\"salt\\\": \\\"1 teaspoon\\\",\\n      \\\"black_pepper\\\": \\\"1/2 teaspoon\\\",\\n      \\\"dried_oregano\\\": \\\"1 1/4 teaspoons\\\",\\n      \\\"dried_basil\\\": \\\"3/4 teaspoon\\\",\\n      \\\"olive_oil\\\": \\\"1 tablespoon + for frying\\\",\\n      \\\"small_onion\\\": \\\"1\\\",\\n      \\\"crushed_tomatoes\\\": \\\"1 (28-ounce) can\\\",\\n      \\\"sugar\\\": \\\"pinch (optional)\\\",\\n      \\\"spaghetti\\\": \\\"12 ounces\\\",\\n      \\\"grated_Parmesan_cheese_garnish\\\": \\\"for garnish\\\",\\n      \\\"chopped_fresh_basil_or_parsley_garnish\\\": \\\"for garnish\\\"\\n    },\\n    \\\"instructions\\\": [\\n      {\\n        \\\"step_number\\\": 1,\\n        \\\"description\\\": \\\"Prepare the Meatballs:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"In a large mixing bowl, combine the ground beef, breadcrumbs, grated Parmesan cheese, minced parsley, minced garlic, egg, salt, black pepper, dried oregano, and dried basil. Mix until well combined.\\\",\\n          \\\"Shape the mixture into meatballs, about 1 to 1.5 inches in diameter.\\\",\\n          \\\"Heat olive oil in a large skillet over medium heat. Cook the meatballs in batches until browned on all sides and cooked through, about 8-10 minutes. Transfer cooked meatballs to a plate and set aside.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 2,\\n        \\\"description\\\": \\\"Make the Tomato Sauce:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"In the same skillet used for cooking the meatballs, add another tablespoon of olive oil if needed. Add the chopped onion and cook until softened, about 3-4 minutes.\\\",\\n          \\\"Stir in the minced garlic and cook for an additional minute until fragrant.\\\",\\n          \\\"Pour in the crushed tomatoes, dried oregano, dried basil, salt, black pepper, and a pinch of sugar if desired. Bring the sauce to a simmer, then reduce the heat to low. Let it simmer for about 15-20 minutes, stirring occasionally.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 3,\\n        \\\"description\\\": \\\"Cook the Spaghetti:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"While the sauce is simmering, cook the spaghetti according to the package instructions until al dente. Once cooked, drain the spaghetti and return it to the pot.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 4,\\n        \\\"description\\\": \\\"Combine and Serve:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Once the sauce has simmered and the meatballs are cooked, add the meatballs to the sauce and let them simmer together for an additional 5 minutes to heat through.\\\",\\n          \\\"Serve the spaghetti topped with the meatballs and sauce. Garnish with grated Parmesan cheese and chopped fresh basil or parsley, if desired.\\\",\\n          \\\"Enjoy your delicious spaghetti and meatballs!\\\"\\n        ]\\n      }\\n    ]\\n  }\\n]\\n\"\r\n    },\r\n    {\r\n      \"role\": \"user\",\r\n      \"content\": \"Eggs, spices, green onions, steak, potatoes, tortilla, bread, sugar, and milk \"\r\n    },\r\n    {\r\n      \"role\": \"assistant\",\r\n      \"content\": \"[\\n  {\\n    \\\"recipe_name\\\": \\\"Steak and Eggs Breakfast Burrito\\\",\\n    \\\"description\\\": \\\"This hearty breakfast burrito is filled with tender steak, scrambled eggs, and sautéed vegetables, all wrapped in a warm tortilla. It's a filling and flavorful way to start your day!\\\",\\n    \\\"ingredients\\\": {\\n      \\\"steak\\\": \\\"8 ounces, thinly sliced\\\",\\n      \\\"eggs\\\": \\\"4\\\",\\n      \\\"olive_oil\\\": \\\"1 tablespoon\\\",\\n      \\\"green_onions\\\": \\\"3, chopped\\\",\\n      \\\"salt\\\": \\\"1/2 teaspoon\\\",\\n      \\\"black_pepper\\\": \\\"1/4 teaspoon\\\",\\n      \\\"tortilla\\\": \\\"4 large\\\",\\n      \\\"optional_toppings\\\": \\\"salsa, sour cream, shredded cheese\\\"\\n    },\\n    \\\"instructions\\\": [\\n      {\\n        \\\"step_number\\\": 1,\\n        \\\"description\\\": \\\"Cook the Steak:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Season the steak with salt and black pepper.\\\",\\n          \\\"In a large skillet, heat olive oil over medium-high heat.\\\",\\n          \\\"Add the steak slices to the skillet and cook until browned and cooked to your desired level of doneness, about 2-3 minutes per side. Transfer the cooked steak to a plate and set aside.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 2,\\n        \\\"description\\\": \\\"Scramble the Eggs:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"In the same skillet, add the chopped green onions and sauté for 2 minutes until softened.\\\",\\n          \\\"In a bowl, beat the eggs with salt and black pepper.\\\",\\n          \\\"Pour the beaten eggs into the skillet with the green onions and cook, stirring constantly, until the eggs are scrambled and cooked to your desired consistency.\\\",\\n          \\\"Remove the skillet from heat.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 3,\\n        \\\"description\\\": \\\"Assemble the Burritos:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Warm the tortillas in a dry skillet or microwave until pliable.\\\",\\n          \\\"Place a portion of the scrambled eggs in the center of each tortilla.\\\",\\n          \\\"Top the eggs with a few slices of cooked steak.\\\",\\n          \\\"Add any optional toppings such as salsa, sour cream, or shredded cheese.\\\",\\n          \\\"Roll up the tortillas, folding in the sides as you go, to form burritos.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 4,\\n        \\\"description\\\": \\\"Serve:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Slice each burrito in half diagonally, if desired, and serve hot.\\\",\\n          \\\"Enjoy your delicious steak and eggs breakfast burrito!\\\"\\n        ]\\n      }\\n    ]\\n  },\\n  {\\n    \\\"recipe_name\\\": \\\"Spicy Roasted Potatoes\\\",\\n    \\\"description\\\": \\\"These spicy roasted potatoes are crispy on the outside, tender on the inside, and packed with flavor. The combination of spices adds a kick to the potatoes, making them a delicious side dish or snack.\\\",\\n    \\\"ingredients\\\": {\\n      \\\"potatoes\\\": \\\"2 pounds, cut into small cubes\\\",\\n      \\\"olive_oil\\\": \\\"2 tablespoons\\\",\\n      \\\"paprika\\\": \\\"1 teaspoon\\\",\\n      \\\"garlic_powder\\\": \\\"1 teaspoon\\\",\\n      \\\"onion_powder\\\": \\\"1/2 teaspoon\\\",\\n      \\\"cayenne_pepper\\\": \\\"1/2 teaspoon\\\",\\n      \\\"salt\\\": \\\"1/2 teaspoon\\\",\\n      \\\"black_pepper\\\": \\\"1/4 teaspoon\\\",\\n      \\\"optional_garnish\\\": \\\"chopped parsley\\\"\\n    },\\n    \\\"instructions\\\": [\\n      {\\n        \\\"step_number\\\": 1,\\n        \\\"description\\\": \\\"Preheat the Oven:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Preheat the oven to 425°F (220°C).\\\",\\n          \\\"Line a baking sheet with parchment paper or foil for easy cleanup.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 2,\\n        \\\"description\\\": \\\"Coat the Potatoes:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"In a large mixing bowl, combine the cubed potatoes, olive oil, paprika, garlic powder, onion powder, cayenne pepper, salt, and black pepper.\\\",\\n          \\\"Toss the potatoes until they are evenly coated with the spice mixture.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 3,\\n        \\\"description\\\": \\\"Roast the Potatoes:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Spread the coated potatoes in a single layer on the prepared baking sheet.\\\",\\n          \\\"Place the baking sheet in the preheated oven and roast for 25-30 minutes, or until the potatoes are golden brown and crispy on the outside, and tender on the inside.\\\",\\n          \\\"Flip the potatoes halfway through cooking to ensure even browning.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 4,\\n        \\\"description\\\": \\\"Garnish and Serve:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Remove the roasted potatoes from the oven and let them cool for a few minutes.\\\",\\n          \\\"Sprinkle with chopped parsley, if desired, for added freshness and color.\\\",\\n          \\\"Serve hot as a side dish or snack.\\\",\\n          \\\"Enjoy your spicy roasted potatoes!\\\"\\n        ]\\n      }\\n    ]\\n  },\\n  {\\n    \\\"recipe_name\\\": \\\"French Toast\\\",\\n    \\\"description\\\": \\\"This classic French toast recipe turns ordinary bread into a decadent and satisfying breakfast or brunch dish. The bread slices are soaked in a sweetened egg and milk mixture, then cooked until golden brown. Serve with maple syrup, powdered sugar, or fresh fruits for a delightful morning treat.\\\",\\n    \\\"ingredients\\\": {\\n      \\\"bread_slices\\\": \\\"6\\\",\\n      \\\"eggs\\\": \\\"3\\\",\\n      \\\"milk\\\": \\\"3/4 cup\\\",\\n      \\\"sugar\\\": \\\"2 tablespoons\\\",\\n      \\\"cinnamon\\\": \\\"1/2 teaspoon\\\",\\n      \\\"vanilla_extract\\\": \\\"1/2 teaspoon\\\",\\n      \\\"butter\\\": \\\"2 tablespoons\\\",\\n      \\\"optional_toppings\\\": \\\"maple syrup, powdered sugar, fresh fruits\\\"\\n    },\\n    \\\"instructions\\\": [\\n      {\\n        \\\"step_number\\\": 1,\\n        \\\"description\\\": \\\"Prepare the Egg Mixture:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"In a shallow bowl or pie dish, whisk together the eggs, milk, sugar, cinnamon, and vanilla extract until well combined.\\\",\\n          \\\"Place the bread slices in a single layer in a separate baking dish or on a large plate.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 2,\\n        \\\"description\\\": \\\"Soak the Bread Slices:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Pour the egg mixture over the bread slices, ensuring that each slice is fully coated.\\\",\\n          \\\"Allow the bread to soak in the mixture for a few seconds on each side, until it becomes slightly saturated.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 3,\\n        \\\"description\\\": \\\"Cook the French Toast:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"In a large skillet or griddle, melt the butter over medium heat.\\\",\\n          \\\"Place the soaked bread slices in the skillet and cook until golden brown, about 2-3 minutes per side.\\\",\\n          \\\"If necessary, work in batches to avoid overcrowding the skillet.\\\",\\n          \\\"Remove the cooked French toast from the skillet and keep it warm while cooking the remaining slices.\\\"\\n        ]\\n      },\\n      {\\n        \\\"step_number\\\": 4,\\n        \\\"description\\\": \\\"Serve:\\\",\\n        \\\"sub_steps\\\": [\\n          \\\"Transfer the cooked French toast to serving plates.\\\",\\n          \\\"Drizzle with maple syrup, sprinkle with powdered sugar, and garnish with fresh fruits, if desired.\\\",\\n          \\\"Serve immediately and enjoy your delicious homemade French toast!\\\"\\n        ]\\n      }\\n    ]\\n  }\\n]\"\r\n    }\r\n  ]"),
            new Message(Role.User, new List<Content>
            {
                "From all the food you see in this picture, what kind of foods can I make?",
                ConvertRawImageToTexture2D()
            })
        };
        Debug.Log("about to submit chat image request");
        var chatRequest = new ChatRequest(messages, model: Model.GPT4_Turbo);
        var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);        
        Debug.Log($"{result.FirstChoice.Message.Role}: {result.FirstChoice} | Finish Reason: {result.FirstChoice.FinishDetails}");
        var response = RemoveEmbeddedCharacters(result.ToString());
        string formattedResponse = CookSessionController.EnsureJsonWrappedWithRecipesKey(response);
        Debug.Log(formattedResponse);
        OnChatGPTInputReceived?.Invoke(formattedResponse);
    }


    public string RemoveEmbeddedCharacters(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        // Remove newline characters
        input = input.Replace("\n", "");

        // Remove carriage return characters
        input = input.Replace("\r", "");

        // Remove tab characters
        input = input.Replace("\t", "");

        input = input.Replace("```json", "");

        input = input.Replace("```", "");

        return input;
    }


}


[System.Serializable]
public class PersonalityType
{
    public string Name;
    public string Description;

    public PersonalityType(string name, string description)
    {
        Name = name;
        Description = description;
    }
}