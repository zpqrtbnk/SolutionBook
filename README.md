SolutionBook
--

Once upon a time there was this excellent extension for Visual Studio, [SolutionStartPage](<https://marketplace.visualstudio.com/items?itemName=Herdo.SolutionStartPage>), which would let you replace the start page with a customizable overview of your solutions. And it was very convenient.

The project lives in its [GitHub repository](<https://github.com/Herdo/SolutionStartPage>).

And then came Visual Studio 2019, which replaced the start page with a start *window*, entirely breaking SolutionStartPage (see issue [#25](<https://github.com/Herdo/SolutionStartPage/issues/25>)). There are a few discussions about the start page on Visual Studio developer community (see [here](<https://developercommunity.visualstudio.com/idea/531110/allow-cutomization-or-extension-of-the-new-start-w.html>)) but it does not seem that Microsoft is going to bring it back anytime soon.

So... this is SolutionBook, a replacement for SolutionStartPage for Visual Studio 2019. It installs a new tool window, which can then be displayed with **View** | **Other Windows** | **SolutionBook** and docked anywhere. I dock it to the left, with the already existing **Toolbox** tool window.

It exposes a treeview of solutions, which can be organized and grouped into folders.

*DISCLAIMERS*

You have to save your changes, using the toolbar button. Saving is *not* automatic. And the refresh toolbar buttons reloads from what has been saved. This is how concurrency between multiple Visual Studio instances is (not) managed.

This was all quickly put together. WPF is hard. There *may* be issues, and the code is probably not the best I've produced. Feel free to comment, suggest, propose changes, etc.