using System.Collections;
using System.Collections.Generic;
//using System.Windows.Forms;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

//using UnityEditor;


public class ClickButton : UIElementStored
{

    UIElementStored theUIElements;

    public void IWasClicked(UnityEngine.UI.Button buttonClicked)
    {
        IWasClicked(buttonClicked.name);
    }

    public async void IWasClicked(string buttonName)
    {
        
        if (theUIElements == null)
            theUIElements = GameObject.Find("UIElementStored").GetComponent<UIElementStored>();
        
        // This is called when we hit the cancel button from the load or save page
        if (buttonName == "CancelButton")
        {
            theUIElements.loadArea.SetActive(false);
            theUIElements.saveArea.SetActive(false);
            return;
        }
        
        // if the textbox has changed from what is currently stored as the answer, we should update the list to being listed as not saved
        if (buttonName != "OpenButtonWithFileName" && theUIElements.currentPageObject.pageTopic != ""
            && (theUIElements.currentPageObject.pageType != 2 && theUIElements.currentPageObject.pageType != 3)
            && theUIElements.currentPageObject.pageAnswer != theUIElements.answerInputField.GetComponent<TMP_InputField>().text)
            theUIElements.listIsSaved = false;

        // This is called when we want to save the file (whether from directly hitting save button or after being asked to save first
        if (buttonName == "SaveButton" || buttonName == "SaveFirst")
        {
            theUIElements.saveFirstArea.SetActive(false);
            theUIElements.saveArea.SetActive(true);

            if (buttonName == "SaveButton")
                theUIElements.currentButtonPressed = "SaveButton";

            return;
        }

        // this will be called after you enter the file name you want to save the project under
        if (buttonName == "SaveButtonWithFileName")
        {
            theUIElements.saveArea.SetActive(false);
            SaveProject(theUIElements.savedFileInputField.GetComponent<TMP_InputField>().text);
            theUIElements.listIsSaved = true;
            buttonName = theUIElements.currentButtonPressed; // change the button back to the original pressed before saving

            // check to see if we also need to show the potential files to load (because clicked open button)
            if (theUIElements.currentButtonPressed == "SaveButton")
                return;
        }

        // this is called after user is asked if they want to save first but then they press "No"
        if (buttonName == "DoNotSave")
        {
            theUIElements.saveFirstArea.SetActive(false);
            if (theUIElements.currentButtonPressed == "OpenButton")
            {
                theUIElements.loadArea.SetActive(true);
                LoadOpenOptions();
                return;
            }
            theUIElements.listIsSaved = true;
            buttonName = theUIElements.currentButtonPressed;
        }

        // called after the open button is pressed, checks if we should save first and if not, then loads the list of files to open
        if (buttonName == "OpenButton")
        {
            theUIElements.currentButtonPressed = "OpenButton";
            theUIElements.showLoadFiles = true;
            theUIElements.saveFirstArea.SetActive(false);
            // if ExitProject returned false, that means that we are showing a save first? dialog before allowing opening
            if (ExitProject())
            {
                theUIElements.loadArea.SetActive(true);
                LoadOpenOptions();
                theUIElements.showLoadFiles = false;
            }
            return;
        }

        // if we have chosed a file to open, we should try to load the data and then set the list as being saved
        if (buttonName == "OpenButtonWithFileName")
        {
            ResetAnswers();
            // try to load the data from the file passed
            if (OpenProject() == false)
            {
                return;
            }
            theUIElements.loadArea.SetActive(false);
            theUIElements.listIsSaved = true;  
        }

        // check to see if this was the last page (if it is the page that contains the 'yes' button and there is nothing saved in the answers, then it is)
        // if it is the last, then we should make sure the user has a chance to save before going on
        if (buttonName == "ExitButton" || buttonName == "NewButton" || (theUIElements.currentPageObject.listOfChoices.Count > 0 && theUIElements.currentPageObject.listOfChoices[0].entryStringName == "Yes"))
        {
            theUIElements.currentButtonPressed = buttonName;
            if (ExitProject())
            {
                ResetAnswers();
                SavedFiles.current = new SavedFiles();
                theUIElements.savedFileInputField.GetComponent<TMP_InputField>().text = "";
            }
            else
                return;
        }
        
        PageObject nextPageObject; // this is the page that we want to transition to

        // check to see which button was pressed and then set the next page the the corresponding option
        if (buttonName == "BackButton")
        { 
            // if the stored previous page is null, that means we hit the back button more than once, so we should use the previous page that is stored in the object
            if (theUIElements.previousPageObject == null)
            {
                if (theUIElements.currentPageObject.previousPage != null)
                    theUIElements.previousPageObject = theUIElements.currentPageObject.previousPage;
                else
                    return; // the case where the back button has been hit multiple times plus trying to go back on the 'How to apply to life' page
            }
            nextPageObject = theUIElements.previousPageObject;
            theUIElements.previousPageObject = null;

        }
        else if (buttonName == "HelpButton")
        {
            nextPageObject = theUIElements.currentPageObject.helpPage;
            theUIElements.previousPageObject = theUIElements.currentPageObject;
        }
        else if (buttonName == "OpenButtonWithFileName")
        {
            nextPageObject = theUIElements.currentPageObject;
            theUIElements.currentPageObject = theUIElements.startScreenPage;
        }
        else if (buttonName == "ExitButton")
        {
            nextPageObject = theUIElements.startScreenPage;
        }
        else if (buttonName == "NewButton")
        {
            nextPageObject = theUIElements.startScreenPage.listOfChoices[0];
        }
        else
        {
            theUIElements.previousPageObject = theUIElements.currentPageObject;
            if (buttonName == "ButtonOption2")
                nextPageObject = theUIElements.currentPageObject.listOfChoices[1];
            else if (buttonName == "ButtonOption3")
                nextPageObject = theUIElements.currentPageObject.listOfChoices[2];
            else
                nextPageObject = theUIElements.currentPageObject.listOfChoices[0];

            if(!theUIElements.listOfAnswers.Contains(theUIElements.currentPageObject))
                theUIElements.listOfAnswers.Add(theUIElements.currentPageObject);

        }
        
        // Show the correct buttons based on the page type of the next page
        ShowCorrectPageButtons(nextPageObject);

        TMP_InputField inputBox = theUIElements.answerInputField.GetComponent<TMP_InputField>();

        // if the current page had a text box, save what was in there. If it had a button, save the entrytext
        if (theUIElements.currentPageObject.pageType == 1)
        {
            theUIElements.currentPageObject.pageAnswer = inputBox.text;
        } else if (theUIElements.currentPageObject.pageType == 2 || theUIElements.currentPageObject.pageType == 3)
        {
            theUIElements.currentPageObject.pageAnswer = nextPageObject.entryStringName;
        }
        
        // if the next page has a text box, update it to show what the user previously entered
        if (nextPageObject.pageType == 1)
        {
            inputBox.text = nextPageObject.pageAnswer;
            //inputBox.Select();
            //inputBox.ActivateInputField();
            //await System.Threading.Tasks.Task.Delay(10);
            //inputBox.MoveToEndOfLine(shift: true, ctrl: false);
        }


        theUIElements.mainText.text = nextPageObject.topText;
        theUIElements.currentPageObject = nextPageObject;
        
    }

    // returns false if we end up opening the "Do you want to save first?" dialog box
    // returns true if there was no need to show the save first button
    public bool ExitProject()
    {
        // check to see if this was the last page (if it is the page that contains the 'yes' button and there is nothing saved in the answers, then it is)
        // if it is the last, then we should make sure the user has a chance to save before going on
        if (theUIElements.listOfAnswers.Count > 1 && theUIElements.listIsSaved == false)
        {
            theUIElements.saveFirstArea.SetActive(true);
            return false;     
        }
        return true;

    }

    // the function creates the string that will be held in the saved file and then calls another function to do the actual saving
    public void SaveProject(string fileNameToSave)
    {
        List<string> lines = new List<string>();

        for (int i = 0; i < theUIElements.listOfAnswers.Count; i++)
        {
            lines.Add(theUIElements.listOfAnswers[i].pageTopic);
            lines.Add(theUIElements.listOfAnswers[i].pageAnswer);
            lines.Add("------------");
        }

        theUIElements.currentPageObject.pageAnswer = theUIElements.answerInputField.GetComponent<TMP_InputField>().text;
        lines.Add(theUIElements.currentPageObject.pageTopic);
        lines.Add(theUIElements.currentPageObject.pageAnswer);
        lines.Add("------------");
        Debug.Log(fileNameToSave);
        SaveLoad.Save(theUIElements.saveDirectory, fileNameToSave, lines);

    }
    
    // this function creates the list of buttons with files that the user can choose from
    public bool LoadOpenOptions()
    {
        string[] saveFiles;
        if (!Directory.Exists(theUIElements.saveDirectory + "/saves"))
        {
            Directory.CreateDirectory(theUIElements.saveDirectory + "/saves");
        }
        
        saveFiles = Directory.GetFiles(theUIElements.saveDirectory + "/saves");

        Button[] buttons = theUIElements.loadArea.GetComponentsInChildren<Button>();

        foreach (Button currentButton in buttons)
        {
            if(currentButton.name != "CancelButton2")
                Destroy(currentButton.gameObject);
        }
        
        for (int i = 0; i < saveFiles.Length; i++)
        {
            
            GameObject buttonGameObject = Instantiate(theUIElements.buttonPrefab, theUIElements.buttonParent.transform);
            
            var index = i;
            Button buttonObject = buttonGameObject.GetComponent<Button>();
            
            buttonObject.onClick.AddListener(() =>
            {
                SaveLoad.Load(theUIElements.saveDirectory, saveFiles[index]); IWasClicked("OpenButtonWithFileName");

            });

            string fileNameToShow = saveFiles[index].Replace(theUIElements.saveDirectory + "/saves", "");
            fileNameToShow = fileNameToShow.Replace("\\", "");
            fileNameToShow = fileNameToShow.Replace("/", "");
            fileNameToShow = fileNameToShow.Replace(".save", "");
            buttonGameObject.GetComponentInChildren<Text>().text = fileNameToShow;
        }

        return true;
    }


    public bool OpenProject()
    {
        string path = "";

        bool startTracking = false;
        List<string> lines = SavedFiles.current.lines;

        string pageTopicFromFile = "";
        string pageAnswerFromFile = "";

        int topicOrAnswer = 0; //if this is 0, the next to be stored is a topic; if 1, then next is an answer

        PageObject pageFound = new PageObject();
        foreach (string line in lines)
        {
            if (line == "------------")
            {
                startTracking = true;
                topicOrAnswer = 0;

                foreach (PageObject pageObject in theUIElements.listOfPages)
                {
                    if (pageObject.pageTopic == pageTopicFromFile)
                    {
                        pageObject.pageAnswer = pageAnswerFromFile;

                        // don't set for the Application page because program won't know which page came before it
                        if (pageObject.pageTopic != "Application: ")
                        {
                            theUIElements.currentPageObject = pageObject;
                            theUIElements.previousPageObject = null;
                        }
                        pageFound = pageObject;
                        break;
                    }
                }

                theUIElements.listOfAnswers.Add(pageFound);
                pageTopicFromFile = "";
                pageAnswerFromFile = "";
                continue;
            }

            if (startTracking == true && topicOrAnswer == 0)
            {
                topicOrAnswer = 1;
                pageTopicFromFile = line;
            }
            else if (startTracking == true && topicOrAnswer == 1)
            {
                if (pageAnswerFromFile == "")
                    pageAnswerFromFile = line;
                else
                    pageAnswerFromFile = pageAnswerFromFile + "\n" + line;
            }

        }
        theUIElements.savedFileInputField.GetComponent<TMP_InputField>().text = SavedFiles.current.fileName;
        return startTracking;
        

    }

    public void ResetAnswers()
    {

        for (int i = 0; i < theUIElements.listOfAnswers.Count; i++)
        {
            theUIElements.listOfAnswers[i].pageAnswer = "";
        }

        theUIElements.answerInputField.GetComponent<TMP_InputField>().text = "";
        theUIElements.listOfAnswers.Clear();

    }

    public void ShowCorrectPageButtons(PageObject nextPageObjectPassed)
    {

        theUIElements.answerInputField.SetActive(false);
        theUIElements.nextButton.SetActive(false);
        theUIElements.topText.SetActive(false);
        theUIElements.exitButton.SetActive(false);
        theUIElements.backButton.SetActive(false);
        theUIElements.saveButton.SetActive(false);
        theUIElements.helpButton.SetActive(false);
        theUIElements.initialOptions.SetActive(false);
        theUIElements.buttonChoices.SetActive(false);
        theUIElements.buttonChoices3.SetActive(false);


        /* pageType == -1 means that it is the start screen
           pageType == 0 means that it is a help page with just a back button
           pageType == 1 means that we should show an input box and a next button
           pageType == 2 means that we should show 2 option buttions
           pageType == 3 means that we should show 3 option buttons
        */
        if (nextPageObjectPassed.pageType == -1)
        {
            theUIElements.initialOptions.SetActive(true);
        } 
        else if (nextPageObjectPassed.pageType == 0)
        {
            theUIElements.backButton.SetActive(true);
        } 
        else if (nextPageObjectPassed.pageType == 1)
        {
            theUIElements.answerInputField.SetActive(true);
            theUIElements.nextButton.SetActive(true);
            theUIElements.topText.SetActive(true);
            theUIElements.exitButton.SetActive(true);
            theUIElements.helpButton.SetActive(true);
            theUIElements.backButton.SetActive(true);
            theUIElements.saveButton.SetActive(true);
        }
        else if (nextPageObjectPassed.pageType == 2)
        {
            theUIElements.topText.SetActive(true);
            theUIElements.buttonChoices.SetActive(true);
            theUIElements.exitButton.SetActive(true);
            theUIElements.helpButton.SetActive(true);

            theUIElements.buttonOption1Text.text = nextPageObjectPassed.listOfChoices[0].entryStringName;
            theUIElements.buttonOption2Text.text = nextPageObjectPassed.listOfChoices[1].entryStringName;
        }
        else if (nextPageObjectPassed.pageType == 3)
        {
            theUIElements.topText.SetActive(true);
            theUIElements.buttonChoices3.SetActive(true);
            theUIElements.exitButton.SetActive(true);
            theUIElements.helpButton.SetActive(true);
            theUIElements.backButton.SetActive(true);
            theUIElements.saveButton.SetActive(true);
        }


    }


}
