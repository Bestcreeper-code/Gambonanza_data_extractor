This mod/tool Runs via the [Bepinex](https://github.com/BepInEx/BepInEx/) Mod loader, you need to install it first(follow the instruction there

This mod adds new keybinds to extract data about the game:
**ctrl+shift+G**: writes the name,desc, rarity and  Unlock Method of every gambit localized to the current language (in a "Name:<name> | Description:<desc> | Rarity:<rarity> | Unlock Method:<unlock method>" format) in **gambits.txt**
**ctrl+shift+S**: writes the name & desc of allthe strains (in a "Name:<name> | Description:<desc>" format in **strains.txt**
**ctrl+shift+R**: writes all sprites(gambits,bosees,ui,etc) to the **Textures/** directory (though files are not renamed right now so their name may be hard to navigate through like «SPR_Armor_sprite.png»)

since **gambits.txt** and **strains.txt** filenames are the same no matter the langauge you may want to rename them in between every language if you want to get data for more than 1 language
