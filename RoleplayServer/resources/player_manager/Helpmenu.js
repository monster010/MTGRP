﻿var menu = new Menu();
API.onServerEventTrigger.connect(function (openHelpMenu, args) {
    switch (openHelpMenu) {
        case "openHelpMenu":
            menu
                .createMenu("Help Menu")
                .addMenuItem("Commands", "", true, true, "showCmds", "showHelpCmds")
                .addMenuItem("Animations", "", true, true, "showCmds", "showAnimCmds")
                .addMenuItem("Police", "", true, true, "showCmds", "showPolCmds")
                .addMenuItem("Admin", "", true, true, "showCmds", "showAdCmds")
                .addMenuItem("Rules", "", true, true, "showCmds", "sRules")
                .addMenuItem("FAQ", "", true, true, "showCmds", "sFAQ")
            break;
    }
});
function showCmds(group) {  
    if (group == "showHelpCmds") {
        API.sendChatMessage('~h~Here is the list of commands availible to you:');
        API.sendChatMessage('/Time, /Stats, /Rp, /B, /Me, /Ame, /(V)ip, /(N)ewbie, /O, /Pm');
    }
    else if (group == "showAnimCmds") {
        API.sendChatMessage('~h~Here is the list of Animations:');
        API.sendChatMessage('/Stopanim, /Hide, /Lookout, /Crowdcontrol, /Investigate, /Drink, /Crossarms, /Idle, /Lean,');
        API.sendChatMessage('/Reach, /Workout, /Smoke, /Binoculars, /Hobo, /Fallover, /Laydown, /Drunk, /Twitchy, /Signal,');
        API.sendChatMessage('/Cheer, /Drugdeal, /Gardening, /Guard, /Jog, /Getjiggy, /Sit, /Mech, /Yoga, /Bonghit,');
        API.sendChatMessage('/Restrained, /MiddleFinger, /Salute, /Slowclap, /Facepalm, /Handsup, /CoverL, /CoverR');
        API.sendChatMessage('~h~~y~VIP Animations');
        API.sendChatMessage('/Clipboard, /Hammer, /Guitar  ');
    }
    else if (group == "showPolCmds") {
        API.sendChatMessage("~h~Here is the list of commands availible to you:");
        API.sendChatMessage("/Arrest, /Frisk, /Cuff, /Uncuff, /Detain, /(M)egaphone");
    }
    else if (group == "showAdCmds") {
        API.sendChatMessage('~h~Here is the list of Admin Commands:');
        API.sendChatMessage('/Agiveweapon, /Spec, /Specoff, /Setadmin, /Spawnveh, /Sethealth, /Setarmour, /Gotopos');
        API.sendChatMessage('~r~TesterCMDs to be removed.');
        API.sendChatMessage('~r~/testeradmin');
    }
    else if (group == "sRules") {
        API.sendChatMessage("To view all the rules head to ~h~MT-Gaming.com~h~ Here some basic server rules:");
        API.sendChatMessage("IC = In character || OOC = Out of character");
        API.sendChatMessage("MG = Metagaming - Using out of character info in character | Using in character info out of character.");
        API.sendChatMessage("DM = Death matching - killing another player without any roleplay reason or any roleplay at all.");
        API.sendChatMessage("PG = Powergaming - Roleplaying the impossible | forcing a player to roleplay situation without giving them a chance.");
    }
    else if (group == "sFAQ") {
        API.sendChatMessage("If your answer isn't there use /n or ask on our forums. ~h~MT-Gaming.com~h~");
        menu
            .createMenu("FAQ")
            .addMenuItem("How to join LSPD?", "", true, true, "ShowFAQ","JoinLSPD")
            .addMenuItem("Where is the bank?", "", true, true, "ShowFAQ", "FBank")
            .addMenuItem("Where do I find X job?", "", true, true, "ShowFAQ", "FJob")
            .addMenuItem("Where do I find X busienss?", "", true, true, "ShowFAQ", "FBusienss")
            .addMenuItem("How do I change my clothes?", "", true, true, "ShowFAQ", "ChangeClothes")
            .addMenuItem("Where do I buy a car?", "", true, true, "ShowFAQ", "BuyCar")
            .addMenuItem("Whats the best job?", "", true, true, "ShowFAQ", "BestJob")
            .addMenuItem("How do I buy VIP?", "", true, true, "ShowFAQ", "BuyVIP")
        ;
    }
}
function ShowFAQ(group) {
    if (group == "JoinLSPD") {
        API.sendChatMessage('Head over to ~h~Mt-Gaming.com~h~ and go to the GTA:V LSPD section to see the requirements');
        API.sendChatMessage('~h~General OOC requirements:~h~ Minimum 25hours, No active warnings, Able to use TeamSpeak.');
        API.sendChatMessage('~h~General IC requirements:~h~ Must be 21 years if age, Valid Drivers licence, No criminal record.');
    }
    else if (group == "FBank") {
        API.sendChatMessage('~r~Currently no /map, ask through /n for help.');
    }
    else if (group == "FJob") {
        API.sendChatMessage('The icons on the map indicate the location of each job.');
    }
    else if (group == "FBusiness") {
        API.sendChatMessage('~r~Currently no /map, ask through /n for help.');
    }
    else if (group == "ChangeClothes") {
        API.sendChatMessage('Head to a clothes store to purchause new clothes.');
    }
    else if (group == "BuyCar") {
        API.sendChatMessage('Head to the dealership on the map to purchause a vehicle inside. Or you can RP with others');
        API.sendChatMessage('to purchause a vehicle off them second hand. (Requires 2 hours playtime)');
    }
    else if (group == "BestJob") {
        API.sendChatMessage('All jobs are made to be very similar with how much money you can earn. Even RPing with others');
        API.sendChatMessage('can be a great way to earn money. We suggest you try them all to see what suits your RP and');
        API.sendChatMessage('is enjoyable to you.');
    }
    else if (group == "BuyVIP") {
        API.sendChatMessage('Head over to ~h~Mt-Gaming.com~h~ Once logged in, click on the ~h~Account upgrades~h~ tab.');
    }
}
API.onUpdate.connect(function () {
    if (menu.menuPool !== null) {
        menu.menuPool.ProcessMenus();
    }
});

function Menu() {
    this.mainMenu = null;
    this.menuPool = API.getMenuPool();

    this.createMenu = function (title, subTitle, isResetKey, posX, posY, anchor) {

        this.destroyMenu();

        title = typeof title !== "undefined" ? title : "Menu";
        subTitle = typeof subTitle !== "undefined" ? subTitle : "";
        isResetKey = typeof isResetKey !== "undefined" ? isResetKey : false;
        anchor = typeof anchor !== "undefined" ? anchor : 6;
        posX = typeof posX !== "undefined" ? posX : 0;
        posY = typeof posY !== "undefined" ? posY : 0;

        this.mainMenu = API.createMenu(title, subTitle, posX, posY, anchor);
        this.menuPool.Add(this.mainMenu);
        this.mainMenu.Visible = true;

        if (isResetKey === true) {
            this.mainMenu.ResetKey(menuControl.Back);
        }

        return this;
    };

    this.addMenuItem = function (title, subTitle, isActivateInvisibleMenu, isActivated, callFunction) {

        title = typeof title !== "undefined" ? title : "MenuItem";
        subTitle = typeof subTitle !== "undefined" ? subTitle : "";
        isActivateInvisibleMenu = typeof isActivateInvisibleMenu !== "undefined" ? isActivateInvisibleMenu : true;
        isActivated = typeof isActivated !== "undefined" ? isActivated : false;
        callFunction = typeof callFunction !== "undefined" ? callFunction : "";

        var args = [].slice.call(arguments).splice(5);
        var uiMenu = this.mainMenu;
        var menuItem = API.createMenuItem(title, subTitle);

        if (isActivated === true) {
            menuItem.Activated.connect(function (menu, item) {

                if (isActivateInvisibleMenu === true)
                    uiMenu.Visible = false;

                eval(callFunction)(args);
            });
        }

        this.mainMenu.AddItem(menuItem);

        return this;
    };

    this.addCheckBoxItem = function (title, subTitle, defaultValue, isActivateInvisibleMenu, isActivated, callFunction) {

        title = typeof title !== "undefined" ? title : "CheckBoxItem";
        subTitle = typeof subTitle !== "undefined" ? subTitle : "";
        defaultValue = typeof defaultValue !== "undefined" ? defaultValue : true;
        isActivateInvisibleMenu = typeof isActivateInvisibleMenu !== "undefined" ? isActivateInvisibleMenu : true;
        isActivated = typeof isActivated !== "undefined" ? isActivated : false;
        callFunction = typeof callFunction !== "undefined" ? callFunction : "";

        var args = [].slice.call(arguments).splice(6);
        var checkBoxItem = API.createCheckboxItem(title, subTitle, defaultValue);
        var uiMenu = this.mainMenu;

        if (isActivated === true) {
            checkBoxItem.CheckboxEvent.connect(function (item, callBack) {
                if (isActivateInvisibleMenu === true)
                    uiMenu.Visible = false;

                eval(callFunction)(callBack, args);
            });
        }

        this.mainMenu.AddItem(checkBoxItem);

        return this;
    };

    this.addCloseButton = function () {

        var uiMenu = this.mainMenu;
        var destroyMenu = this.destroyMenu();
        var menuItem = API.createMenuItem("~r~Close", "");

        menuItem.Activated.connect(function (menu, item) {
            uiMenu.Visible = false;
            destroyMenu();
        });
        this.mainMenu.AddItem(menuItem);

        return true;
    };

    this.callClientFunction = function (functionName /*, args */) {
        var args = [].slice.call(arguments).splice(1);
        eval(functionName)(args);
    };

    this.callServerFunction = function (functionName /*, args */) {
        var args = [].slice.call(arguments).splice(1);
        API.triggerServerEvent(functionName, args);
    };

    this.destroyMenu = function () {
        if (this.mainMenu !== null)
            this.mainMenu.Visible = false;

        this.mainMenu = null;
        this.menuPool = null;
        this.menuPool = API.getMenuPool();

        return true;
    };
}