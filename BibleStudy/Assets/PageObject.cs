using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageObject
{
    public string entryStringName; // the string on the button that is pressed to get here
    public string pageTopic; // the topic that is shown in saved file
    public string topText; // the text that is shown on the page's screen
    public int pageType; // determines which buttons to show
    public PageObject previousPage, helpPage;
    public string pageAnswer; // the answer that the user puts for this page
    public List<PageObject> listOfChoices; // the pages that you can get to based on the buttons that are pressed

    public PageObject()
    {
        entryStringName = null;
        pageTopic = "";
        topText = "";
        pageType = 0;
        previousPage = null;
        helpPage = null;
        listOfChoices = new List<PageObject>();
        pageAnswer = "";
    }

    //used for help pages
    public PageObject(string topTextPassed, int pageTypePassed, PageObject previousPagePassed)
    {
        entryStringName = null;
        pageTopic = null;
        topText = topTextPassed;
        pageType = pageTypePassed;
        previousPage = previousPagePassed;
        listOfChoices = new List<PageObject>();
        pageAnswer = null;
    }

    // used for most pages
    public PageObject(string entryStringNamePassed, string pageTopicPassed, string topTextPassed, int pageTypePassed, PageObject previousPagePassed)
    {
        entryStringName = entryStringNamePassed;
        pageTopic = pageTopicPassed;
        topText = topTextPassed;
        pageType = pageTypePassed;
        previousPage = previousPagePassed;
        listOfChoices = new List<PageObject>();
        pageAnswer = "";
    }

    public void AddPageObject(PageObject newPageObject)
    {
        listOfChoices.Add(newPageObject);
    }

    public void AddPageAnswer(string pageAnswerPassed)
    {
        pageAnswer = pageAnswerPassed;
    }

    public string GetPageTopic()
    {
        return pageTopic;
    }
}
