using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;



public class UIElementStored : MonoBehaviour
{
    // access objects on the GUI
    public GameObject   saveButton, exitButton, backButton, helpButton, initialOptions, loadArea, saveArea, saveFirstArea, savedFileInputField, inputAndNext,
                        buttonChoicesHorizontal, buttonChoicesVertical, buttonChoices3Horizontal, buttonChoices3Vertical,
                        answerInputFieldHorizontal, nextButtonHorizontal, topTextHorizontal, answerInputFieldVertical, nextButtonVertical, topTextVertical, answerInputFieldSquare, nextButtonSquare, topTextSquare;
    
    public TextMeshProUGUI mainTextHorizontal, buttonOption1TextHorizontal, buttonOption2TextHorizontal, mainTextVertical, buttonOption1TextVertical, buttonOption2TextVertical, mainTextSquare;
    public Font font;
    public Texture boxTexture;
    public GameObject buttonPrefab;
    public GameObject buttonParent;

    // some variables to keep track of where we are in the program - #F0D4C8
    public GameObject buttonChoices, buttonChoices3, answerInputField, nextButton, topText;
    public TextMeshProUGUI mainText, buttonOption1Text, buttonOption2Text;
    public PageObject currentPageObject, previousPageObject, startScreenPage;
    public bool listIsSaved = true;
    public bool showLoadFiles = false;

    public List<PageObject> listOfPages;
    public List<PageObject> listOfAnswers;


    public string currentOrientation, currentButtonPressed;
    public string saveDirectory = "";


    void Start()
    {
        
        if (saveDirectory == "" || !Directory.Exists(saveDirectory))
            saveDirectory = Application.persistentDataPath;
            
        
        // only do the setting up code if it has not been done before
        if (answerInputField != null)
        {
            // set up the initial screen, hiding objects that aren't initially shown
            mainText.text = "";
            answerInputFieldVertical.SetActive(false);
            answerInputFieldHorizontal.SetActive(false);
            answerInputFieldSquare.SetActive(false);
            nextButtonVertical.SetActive(false);
            nextButtonHorizontal.SetActive(false);
            nextButtonSquare.SetActive(false);
            topTextVertical.SetActive(false);
            topTextHorizontal.SetActive(false);
            topTextSquare.SetActive(false);
            backButton.SetActive(false);
            saveButton.SetActive(false);
            exitButton.SetActive(false);
            helpButton.SetActive(false);
            buttonChoicesHorizontal.SetActive(false);
            buttonChoicesVertical.SetActive(false);
            buttonChoices3Horizontal.SetActive(false);
            buttonChoices3Vertical.SetActive(false);
            loadArea.SetActive(false);
            saveArea.SetActive(false);
            saveFirstArea.SetActive(false);

            SetupScreen();
            currentButtonPressed = "";

            // create list of pages
            PageObject applyPage;
            PageObject beginningPage, firstChoicePage, secondChoicePage, thirdChoicePage, tempPageStorage, helpPage;
            listOfPages = new List<PageObject>(); // this will contain all possible pages we can get to that might need to be saved
            listOfAnswers = new List<PageObject>(); // this will contain all the pages that we actually added answers to


            // start by adding the last couple of pages that all paths with link back to
            applyPage = new PageObject("Next", "Application: ", "How can you apply what was read?", 1, null);
            listOfPages.Add(applyPage);
            helpPage = new PageObject("No help page yet", 0, applyPage);
            firstChoicePage = new PageObject("Next", "", "Do you want to start again?", 2, applyPage);
            AddChoicesToList(applyPage, helpPage, firstChoicePage, null, null);

            beginningPage = firstChoicePage;
            firstChoicePage = new PageObject("Yes", "Study By: ", "How would you like to study?", 2, beginningPage);
            listOfPages.Add(firstChoicePage);
            startScreenPage = new PageObject("No", "", "", -1, null);
            AddChoicesToList(startScreenPage, helpPage, firstChoicePage, firstChoicePage, firstChoicePage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, startScreenPage, null);

            // the page we just created is actually the first page we start on, so set it to the current page
            currentPageObject = startScreenPage;

            // Add the rest of the pages
            beginningPage = firstChoicePage;
            firstChoicePage = new PageObject("By Passage", "Passage: ", "Pick a passage", 1, beginningPage);
            listOfPages.Add(firstChoicePage);
            secondChoicePage = new PageObject("By Topic", "Topic: ", "Pick a topic", 1, beginningPage);
            listOfPages.Add(secondChoicePage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, secondChoicePage, null);
            tempPageStorage = secondChoicePage; // we will add this option's list values later, so store for now

            beginningPage = firstChoicePage;
            firstChoicePage = new PageObject("Next", "Previous Information: ", "What happened before the passage? (or if the first passage in a book, who wrote it and why?)", 1, beginningPage);
            listOfPages.Add(firstChoicePage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, null, null);

            beginningPage = firstChoicePage;
            firstChoicePage = new PageObject("Next", "Passage Summary: ", "Read the passage. What is it about?", 1, beginningPage);
            listOfPages.Add(firstChoicePage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, null, null);

            beginningPage = firstChoicePage;
            firstChoicePage = new PageObject("Next", "Original Audience Application: ", "Who is the original audience and how would the passage apply to them?", 1, beginningPage);
            listOfPages.Add(firstChoicePage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, null, null);

            beginningPage = firstChoicePage;
            firstChoicePage = new PageObject("Next", "Parallel Passages: ", "Are there any parallel passages/cross references? If so, what additional information to they provide?", 1, beginningPage);
            listOfPages.Add(firstChoicePage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, null, null);

            beginningPage = firstChoicePage;
            firstChoicePage = new PageObject("Next", "My Questions: ", "What questions came to mind as you read?", 1, beginningPage);
            listOfPages.Add(firstChoicePage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, null, null);

            beginningPage = firstChoicePage;
            firstChoicePage = new PageObject("Next", "Type of study: ", "Which Study Path would you like to study?", 3, beginningPage);
            listOfPages.Add(firstChoicePage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, null, null);

            beginningPage = firstChoicePage;
            firstChoicePage = new PageObject("Word Study", "Words of interest: ", "What words stuck out to you in the passage?", 1, beginningPage);
            listOfPages.Add(firstChoicePage);
            secondChoicePage = new PageObject("Verse Study", "Repetion: ", "Are there any words/thoughts repeated multiple times?", 1, beginningPage);
            listOfPages.Add(secondChoicePage);
            thirdChoicePage = new PageObject("Question Study", "Focus Question: ", "Pick a question that you had.", 1, beginningPage);
            listOfPages.Add(thirdChoicePage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, secondChoicePage, thirdChoicePage);

            beginningPage = firstChoicePage;
            firstChoicePage = new PageObject("Next", "Other instances: ", "Where else are they used in the Bible and do they mean the same thing in those passages?", 1, beginningPage);
            listOfPages.Add(firstChoicePage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, null, null);

            beginningPage = firstChoicePage;
            AddChoicesToList(beginningPage, helpPage, applyPage, null, null);

            beginningPage = secondChoicePage;
            firstChoicePage = new PageObject("Next", "Cause and Effects: ", "Are there any cause and effects occurring?", 1, beginningPage);
            listOfPages.Add(firstChoicePage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, null, null);

            beginningPage = firstChoicePage;
            firstChoicePage = new PageObject("Next", "Comparisions: ", "Are there any comparisons/contrasts within the verses?", 1, beginningPage);
            listOfPages.Add(firstChoicePage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, null, null);

            beginningPage = firstChoicePage;
            AddChoicesToList(beginningPage, helpPage, applyPage, null, null);

            beginningPage = thirdChoicePage;
            firstChoicePage = new PageObject("Next", "Other passages: ", "Are there any other passages that address it?", 1, beginningPage);
            listOfPages.Add(firstChoicePage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, null, null);

            beginningPage = firstChoicePage;
            firstChoicePage = new PageObject("Next", "Other people's answers: ", "How do other people address it and are their answers Biblical?", 1, beginningPage);
            listOfPages.Add(firstChoicePage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, null, null);

            beginningPage = firstChoicePage;
            AddChoicesToList(beginningPage, helpPage, applyPage, null, null);

            beginningPage = tempPageStorage;
            firstChoicePage = new PageObject("Next", "Questions: ", "What are some questions that you have about the topic?", 1, beginningPage);
            listOfPages.Add(firstChoicePage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, null, null);

            beginningPage = firstChoicePage;
            firstChoicePage = new PageObject("Next", "Synonyms: ", "Think of some synonyms of your topic", 1, beginningPage);
            listOfPages.Add(firstChoicePage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, null, null);

            beginningPage = firstChoicePage;
            firstChoicePage = new PageObject("Next", "Antonyms: ", "Think of some antonyms of your topic", 1, beginningPage);
            listOfPages.Add(firstChoicePage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, null, null);

            beginningPage = firstChoicePage;
            firstChoicePage = new PageObject("Next", "Other Passages of Interest: ", "Look in a concordance or google to find verses that contain your topic (look up synonyms and antonyms as well)", 1, beginningPage);
            listOfPages.Add(firstChoicePage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, null, null);

            beginningPage = firstChoicePage;
            firstChoicePage = new PageObject("Next", "", "Read those passages (both before and after to get context)", 1, beginningPage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, null, null);

            beginningPage = firstChoicePage;
            firstChoicePage = new PageObject("Next", "Summary: ", "Summarize what you have read:", 1, beginningPage);
            listOfPages.Add(firstChoicePage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, null, null);

            beginningPage = firstChoicePage;
            firstChoicePage = new PageObject("Next", "My answers and support: ", "Based on what you read, how would you respond to the questions you gathered before? What verses support your points?", 1, beginningPage);
            listOfPages.Add(firstChoicePage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, null, null);

            beginningPage = firstChoicePage;
            firstChoicePage = new PageObject("Next", "Other source answers: ", "If you still have questions, how do other people answer them? Are those answers supported in scripture?", 1, beginningPage);
            listOfPages.Add(firstChoicePage);
            AddChoicesToList(beginningPage, helpPage, firstChoicePage, null, null);

            beginningPage = firstChoicePage;
            AddChoicesToList(beginningPage, helpPage, applyPage, null, null);

        }


    }

    void Update()
    {
        if (answerInputField != null)
        {
            if (Screen.width < Screen.height)
            {
                if (currentOrientation != "Portrait")
                {
                    ChangeOrientation();
                }
            }
            else
            {
                if (currentOrientation != "Landscape")
                {
                    ChangeOrientation();
                }
            }
        }
    }

    public void ChangeOrientation()
    {
        if (currentPageObject.pageType == 2)
        {
            topText.SetActive(false);
            buttonChoices.SetActive(false);
            SetupScreen();
            topText.SetActive(true);
            buttonChoices.SetActive(true);
        }
        else if (currentPageObject.pageType == 3)
        {
            topText.SetActive(false);
            buttonChoices3.SetActive(false);
            SetupScreen();
            topText.SetActive(true);
            buttonChoices3.SetActive(true);
        }
        else if (currentPageObject.pageType == 1)
        {
            topText.SetActive(false);
            answerInputField.SetActive(false);
            nextButton.SetActive(false);
            SetupScreen();
            topText.SetActive(true);
            answerInputField.SetActive(true);
            nextButton.SetActive(true);
        }
    }

    public void SetupScreen()
    {
        

        double width = Screen.width;
        double height = Screen.height;
        double ratio;

        string currentText = mainText.text;
        if (Screen.width < Screen.height)
        {
            currentOrientation = "Portrait";
            ratio = width / height;
            buttonChoices = buttonChoicesVertical;
            buttonOption1Text = buttonOption1TextVertical;
            buttonOption2Text = buttonOption2TextVertical;
            buttonChoices3 = buttonChoices3Vertical;

            answerInputField = answerInputFieldVertical;
            nextButton = nextButtonVertical;
            topText = topTextVertical;
            mainText = mainTextVertical;

        }
        else
        {
            currentOrientation = "Landscape";
            ratio = height / width;
            buttonChoices = buttonChoicesHorizontal;
            buttonOption1Text = buttonOption1TextHorizontal;
            buttonOption2Text = buttonOption2TextHorizontal;
            buttonChoices3 = buttonChoices3Horizontal;

            if (ratio >= .5)
            {
                answerInputField = answerInputFieldSquare;
                nextButton = nextButtonSquare;
                topText = topTextSquare;
                mainText = mainTextSquare;
            }
            else
            {
                answerInputField = answerInputFieldHorizontal;
                nextButton = nextButtonHorizontal;
                topText = topTextHorizontal;
                mainText = mainTextHorizontal;
            }
            
        }
        mainText.text = currentText;

    }

    public void AddChoicesToList(PageObject beginningPage, PageObject helpPage, PageObject firstChoicePage, PageObject secondChoicePage, PageObject thirdChoicePage)
    {
        beginningPage.helpPage = helpPage;
        beginningPage.AddPageObject(firstChoicePage);

        if (secondChoicePage != null)
        {
            beginningPage.AddPageObject(secondChoicePage);
            if (thirdChoicePage != null)
                beginningPage.AddPageObject(thirdChoicePage);
        }
    }


}
