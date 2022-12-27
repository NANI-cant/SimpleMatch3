mergeInto(LibraryManager.library, {

    ShowYandexAd: function () {
        window.ysdk.adv.showFullscreenAdv();
    },

    SaveExtern: function(progress){
        var progressString = UTF8ToString(progress);
        var myObj = JSON.parse(progressString);
        console.log(progressString);
        
        player
            .setData(myObj)
            .then(()=>{console.log('saved')})
            .catch(()=>{console.log('unsaved')});
    },

    LoadExtern: function(){
        player.getData().then(data=>{
            const myJSON = JSON.stringify(data);
            myGameInstance.SendMessage('Yandex','SetProgress', myJSON);    
        });
    },
    
    Log: function(log){
        console.log(UTF8ToString(log));
    },

});