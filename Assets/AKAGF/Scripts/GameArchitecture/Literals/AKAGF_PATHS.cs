
namespace AKAGF.GameArchitecture.Literals {

    public struct AKAGF_PATHS  {

        //PATHS
        public const string AKAGF_BASE_PATH = "Assets/AKAGF/";
        public const string DATA_PATH = "Assets/AKAGF_data/";
        public const string SINGLETONS_PATH = "Singletons/";
        public const string SINGLETONS_FULLPATH = DATA_PATH + SINGLETONS_PATH;
        public const string INVENTORY_ITEM_LIST_PATH = DATA_PATH + "ItemLists/";
        public const string SCRIPTABLE_VARS_PATH = DATA_PATH + "ScriptableVars/";
        public const string LOCALIZATION_PATH = DATA_PATH + "Localization/";
        public const string LOCALIZATION_GROUPS_PATH = LOCALIZATION_PATH + "LocalizedTextsGroupsAssests/";
        public const string LOCALIZATION_GROUPS_FILES_PATH = LOCALIZATION_PATH + "LocalizedTextsGroupsFiles/";
        public const string QUESTS_LIST_PATH = DATA_PATH + "QuestsLists/";

        // HTTP PATHS
        public const string ISO_LANGUAGES_WEB_FILE_PATH = "https://pastebin.com/raw/n5BqBUQM";

        // SCRIPTABLE ASSETS NAMES
        public const string ALLSCENES_ASSET_NAME = "AllGameScriptableScenes";
        public const string NEW_SCRIPTABLE_SCENE_BASE_NAME = "ScriptableScene";
        public const string ALLCONDITIONS_ASSET_NAME = "AllGameConditions";
        public const string NEW_INVENTORY_ITEM_LIST_BASE_NAME = "InventoryItemList.asset";
        public const string ALLSAVESDATA_ASSET_NAME = "AllGameSavesData";
        public const string ALLLANGUAGES_ASSET_NAME = "AllGameLanguages";
        public const string ISO_LANGUAGES_FILE_NAME = "ISO_Language_Codes.json";
        public const string LOCALIZED_TEXTS_GROUP_NAME = "LocalizedTextsGroup.asset";
        public const string NEW_QUESTS_LIST_BASE_NAME = "QuestsList.asset";
        // public const string LOCALIZED_SINGLE_TEXT_NAME = "SingleLocalizedText.asset";

        // Unity Assets Menu Paths
        public const string ASSESTS_CREATE_MENU = "Assets/Create/";
        public const string AKAGF_MENU_FULL_PATH = ASSESTS_CREATE_MENU + "AKA Game Framework Assets/";
        public const string AKAGF_MENU_BASE_PATH = "AKA Game Framework Assets/";
        public const string SCENE_MNGMNT_MENU_PATH = "Scene Management/";
        public const string GAME_FLOW_MENU_PATH = "Game Flow/";
        public const string INVENTORY_MENU_PATH = "Inventory/";
        public const string SCRIPTABLE_VARS_MENU_PATH = AKAGF_MENU_FULL_PATH + "Scriptable Variables/";
        public const string LOCALIZATION_MENU_PATH = AKAGF_MENU_FULL_PATH + "Localization/";
        public const string QUEST_MENU_PATH = "Quest System/";

        // Unity Assets Menu Names
        public const string ALLGAMELANGUAGES_MENU_NAME = "Create All Game Languages" ;
        public const string LOCALIZED_TEXTS_GROUP_MENU_NAME = "Create Localized Texts Group";
        public const string ALLSCENES_MENU_NAME = "Create All Game Scenes";
        public const string ALLCONDITIONS_MENU_NAME = "Create All Game Conditions";
        public const string ALLSCENESAVESDATAS_MENU_NAME = "Create All Scenes Persistent Data";
        public const string INVENTORY_ITEM_LIST_MENU_NAME = "Create Inventory Items Group";
        public const string QUESTS_LIST_MENU_NAME = "Create Quests List";

    }
}
