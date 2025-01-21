var dotnetHelper = null;
var spreadsheet = null;
var currentObjectId = null;
var defaultData = null;
function SaveSpreadSheet() {
	spreadsheet.saveAsJson().then((Json) => {
		var file = JSON.stringify({

			JSONData: JSON.stringify(Json.jsonObject.Workbook),
			ContentType: "Xlsx",
			PDFLayoutSettings: JSON.stringify({ FitSheetOnOnePage: true }),
			VersionType: "Xlsx"
		});
		window.JSFunctions.CreateSpreadSheetFromFile(file);


	})

}
//function GetFileData() {
//	return new Promise((resolve, reject) => {
//		spreadsheet.saveAsJson().then((Json) => {
//			try {
//				var file = JSON.stringify({
//					JSONData: JSON.stringify(Json.jsonObject.Workbook),
//					ContentType: "Xlsx",
//					PDFLayoutSettings: JSON.stringify({ FitSheetOnOnePage: true }),
//					VersionType: "Xlsx"
//				});
//				console.log("File obtained" + file);
//				resolve(file);
//			} catch (error) {
//				console.error("Error " + error);
//				reject(error);
//			}
//		}).catch(error => {
//			reject(error);
//		});
//	});
//}
function GetFileData() {
	return new Promise((resolve, reject) => {
		const chunkSize = 1024 * 1024; // 1 MB
		spreadsheet.saveAsJson().then((Json) => {
			const fileContent = JSON.stringify({
				JSONData: JSON.stringify(Json.jsonObject.Workbook),
				ContentType: "Xlsx",
				PDFLayoutSettings: JSON.stringify({ FitSheetOnOnePage: true }),
				VersionType: "Xlsx"
			});

			const totalChunks = Math.ceil(fileContent.length / chunkSize);
			const chunks = [];

			for (let i = 0; i < totalChunks; i++) {
				const chunk = fileContent.slice(i * chunkSize, (i + 1) * chunkSize);
				chunks.push(chunk);
			}

			resolve(chunks);
		}).catch(error => {
			reject(error);
		});
	});
}

window.JSFunctions = {
	//my c# functions
	CreateSpreadSheetFromFile: function (file) {
		if (dotnetHelper != null)
			return dotnetHelper.invokeMethodAsync('GetSpreadSheetFile', file)
				.then(result => {
					console.log("Success", result);
				})
				.catch(error => {
					console.error("Error ", error);
				});
		return null;
	},
	IsModifiedEventHandler: function (IsModified) {
		if (dotnetHelper != null)
			return dotnetHelper.invokeMethodAsync('SetSpreadSheetModified', IsModified);
		return null;
	},
	RenderSpreadsheet: function (spreadsheetElem, fileStream, dotnetHelp) {
		dotnetHelper = dotnetHelp;

		// Ensure the element reference is available
		if (!spreadsheetElem) {
			console.error("Spreadsheet element reference is not available.");
		}

		// Ensure the file stream is not empty
		if (!fileStream) {
			console.error("File stream is not available.");
		}

		// Initialize the spreadsheet component
		spreadsheet = new ej.spreadsheet.Spreadsheet({
			allowOpen: true,
			allowSave: true,
			height: '670px',
			sheets: [{
				ranges: [{
					showFieldAsHeader: true
				}]
			}],
			openUrl: 'https://ej2services.syncfusion.com/production/web-services/api/spreadsheet/open',
			saveUrl: 'https://ej2services.syncfusion.com/production/web-services/api/spreadsheet/save',
			created: function () {
				//Open the spreadsheet from the JSON file stream
				//window.JSFunctions.IsModifiedEventHandler(true);
				spreadsheet.openFromJson({ file: fileStream });
			},
			actionComplete: function (args) {
				if (args.action == 'import' || args.action == 'resize' || args.action == 'delete' || args.action == 'format') {
					window.JSFunctions.IsModifiedEventHandler(true);
				}
			},
			cellSave: function (args) {
				window.JSFunctions.IsModifiedEventHandler(true);
			},
			resize: function (args) {
				window.JSFunctions.IsModifiedEventHandler(true);
			}
		});

		// Append the spreadsheet to the element
		spreadsheet.appendTo(spreadsheetElem);
	}
};