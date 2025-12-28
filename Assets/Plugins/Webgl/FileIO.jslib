mergeInto(LibraryManager.library, {
  DownloadTsvFile: function (filenamePtr, textPtr) {
    const filename = UTF8ToString(filenamePtr);
    const text = UTF8ToString(textPtr);

    const blob = new Blob([text], { type: "text/plain" });
    const link = document.createElement("a");

    link.href = URL.createObjectURL(blob);
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    URL.revokeObjectURL(link.href);
  }
});

mergeInto(LibraryManager.library, {
  UploadTextFile: function (gameObjectNamePtr, callbackMethodPtr) {
    const gameObjectName = UTF8ToString(gameObjectNamePtr);
    const callbackMethod = UTF8ToString(callbackMethodPtr);

    console.log("UploadTextFile called");
    console.log("Target:", gameObjectName, callbackMethod);

    const input = document.createElement("input");
    input.type = "file";
    input.accept = ".tsv";

    input.onchange = () => {
      const file = input.files[0];
      if (!file) return;

      console.log("File selected:", file.name);

      const reader = new FileReader();
      reader.onload = () => {
        const data = 
        {
          fileName: file.name,
          fileText: reader.result
        };

        console.log("File read, sending to Unity");
        SendMessage(gameObjectName, callbackMethod, JSON.stringify(data));
      };
      reader.readAsText(file);
    };

    input.click();
  }
});
