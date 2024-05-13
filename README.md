# Flaivor

## Inspiration
Flaivor emerged from our genuine excitement about the possibilities with AI and mixed reality, inspired by the latest from Meta's presence platform. We saw a chance to create something truly unique in the kitchen space—turning cooking into an interactive, high-tech experience.

## What it does
Flaivor is a mixed reality AI cooking assistant designed to transform the way people cook at home. By leveraging advanced AI and mixed reality technology, Flaivor offers an interactive, hands-on cooking experience right in your kitchen. Users start by either taking a photo of their ingredients or directly telling Flaivor what they have on hand. The system then processes this information and suggests recipes that not only match the available ingredients but also cater to personal preferences and dietary restrictions.

The interface features Martha, our AI cooking assistant, who enhances the user experience by answering questions and assisting with various cooking tasks, such as setting timers and offering substitutions. Martha’s presence makes the cooking process smoother and more interactive, as she can provide real-time help and guidance every step of the way.

## How we built it
The development of Flaivor was hands-on and iterative. We started by setting up an AI that could handle voice commands and give cooking advice. Then we tied this AI into a mixed reality environment where users could virtually handle ingredients and follow recipes in a dynamic 3D space, capitalizing on all the available interaction modalities with the Meta SDKs. We went through many versions to get the balance right—making sure the technology was helpful without getting in the way of the cooking itself.

## Setup

Setting up the AI NPC Framework in your Unity project is straightforward. Follow these steps to get started:

#### Running Sample

1. **Clone the Repository**: Clone this repository to your local machine using `git clone`.

2. **API Key Configuration**: Obtain API keys for the ChatGPT API and Eleven Labs API (provided by RageAgainstThePixel on GitHub) and configure them in your project.

3. **MainAR**: Run the main scene, ensure that API keys are generated from previous step.

## Environment
- Meta Quest Pro/3/2
- Unity Editor 2022.3.26f
## Dependencies
- [Meta Interaction SDK](link-to-SDK)
- [Meta Passthrough SDK](link-to-SDK)
- [Meta Voice Dictation SDK](https://developer.oculus.com/documentation/unity/voice-sdk-overview/)
- [ChatGPT API](https://github.com/RageAgainstThePixel/com.openai.unity)
- [Eleven Labs API](https://github.com/RageAgainstThePixel/com.rest.elevenlabs)
- [Meta Quest Screenshot Loader](https://github.com/t-34400/MetaQuestScreenshotLoader/)



## Challenges we ran into
We encountered several challenges throughout our project. Formatting the Meta Interaction SDK to meet our needs required frequent iteration. We also faced latency issues with the OpenAI API, which we are working to improve with task parallelization and the latest models. Instantiating panels in front of the user proved difficult, especially ensuring they remained in the same spot when navigating forward or backward. Subscribing to voice events from the Voice SDK posed additional challenges, and integrating the Scene SDK took more time than anticipated -- we plan to revisit that integration. We also struggled with creating sticky menu items, such as floating panels that follow the user. These issues provided valuable learning experiences, and we aim to address them in future iterations.

## Accomplishments that we're proud of
**Intuitive User Experience:** 

We've designed Flaivor to be as intuitive and user-friendly as possible, featuring a guided step-by-step approach that follows along with the user throughout the cooking process. This method ensures that users are never lost or confused about the next steps, making cooking less daunting and more enjoyable. Additionally, Martha, our AI cooking assistant, enhances this experience by providing real-time support and interaction. She can answer questions, help set timers, and provide guidance on cooking techniques, making the whole process feel like you have a professional chef right beside you in the kitchen.

**Advanced Ingredient Recognition and Dynamic Recipe Generation:** 

One of our major technical achievements is the development of a sophisticated ingredient recognition system. Flaivor can accurately identify a wide array of ingredients from just a photo, which is pivotal for suggesting recipes based on what's available. Moreover, our system is capable of dynamically generating recipes, providing endless possibilities for culinary exploration. This feature simplifies the process of searching for recipes online, where often the challenge is finding recipes that match the ingredients you have on hand. With Flaivor, users can instantly receive a variety of recipe options tailored to the ingredients they already possess, eliminating the frustration of incomplete ingredient lists.

**Adaptable Cooking Guidance:** 

Martha, our AI assistant, plays a key role in adapting cooking instructions to the user's needs. While we haven't yet tested this with real users, the potential for personalized cooking is significant. Users can talk to Martha to get tips on adjusting recipes according to their cooking skills or taste preferences, making each recipe flexible and customizable.

**Educational Potential:** 

We're excited about the educational possibilities with Flaivor. Martha is equipped to guide users through recipes and provide useful culinary facts and tips, aiming to make each cooking session both informative and enjoyable.

## What we learned
Building Flaivor was a crash course in integrating complex technologies. We had to figure out how to make AI understand and assist with cooking without frustrating users with tech jargon or clumsy interactions. The project sharpened our skills in designing user-friendly interfaces and taught us a lot about real-time computing challenges.

## What's next for Spatial Agents
Moving forward, the team behind Flaivor is excited to expand and refine the project with the hope of transforming it from a hackathon prototype into a tangible product for everyday kitchens. 

During development, we discussed moonshot ideas. One of the core inspirations behind this idea was to create shared mixed reality experiences locally and remotely. We love the idea of long-distance couples cooking together with the Chef. As such, we plan to investigate Spatial Anchors and Photon multiplayer support to realize that vision.

Beyond Meta's SDK capabilities, we're looking to perfect the user experience, enhance Martha's AI capabilities, and fully integrate the mixed reality environment to make cooking more interactive and fun. Our focus will be on making the technology accessible and engaging, ensuring it can be a helpful companion in the kitchen for everyone from cooking novices to seasoned chefs.
