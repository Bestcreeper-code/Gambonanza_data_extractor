# This mod/tool Runs via the [Bepinex](https://github.com/BepInEx/BepInEx/) Mod loader, you need to install it first(follow the instruction there)

This mod adds new keybinds to extract data about the game:

- **ctrl+shift+G**: writes the name,desc, rarity and  Unlock Method of every gambit localized to the current language (in a "Name:{name} | Description:{desc} | Rarity:{rarity} | Unlock Method:{unlock method}" format) in **gambits{suffix}.txt**

- **ctrl+shift+S**: writes the name & desc of allthe strains (in a "Name:{name} | Description:{desc}" format in **strains{suffix}.txt**

- **ctrl+shift+R**: writes all sprites(gambits,bosees,ui,etc) to the **Textures/** directory (though files are not renamed right now so their name may be hard to navigate through like «SPR_Armor_sprite.png»)

- **ctrl+shift+T**: writes the whole translation json file to **language{suffix}.json**

#### {suffix} is going to be the short for whaterver the current language is (you need to change the current langauge in the ingame settings to get the files for another language)

also fun addition: the rare main menu event is enforced every time 