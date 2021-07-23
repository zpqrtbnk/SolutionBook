SolutionBook
--

Once upon a time there was this excellent extension for Visual Studio, [SolutionStartPage](<https://marketplace.visualstudio.com/items?itemName=Herdo.SolutionStartPage>), 
which would let you replace the start page with a customizable overview of your solutions. And it was very convenient. You can find sources for the project in its own
[GitHub repository](<https://github.com/Herdo/SolutionStartPage>).

And then came Visual Studio 2019, which replaced the start page with a start *window*, entirely breaking SolutionStartPage (see issue 
[#25](<https://github.com/Herdo/SolutionStartPage/issues/25>)). There were a few discussions about the start page on Visual Studio developer community
(see [here](<https://developercommunity.visualstudio.com/idea/531110/allow-cutomization-or-extension-of-the-new-start-w.html>)) but Microsoft never really brought it back.

Visual Studio now proposes different ways to manage recent and frequently used solutions, but none that really pleases me. So... this is **SolutionBook**, a solution
management extension for Visual Studio 2019+. It installs a new tool window, which can then be displayed with **View** | **Other Windows** | **SolutionBook** 
and docked anywhere. I like to dock it to the left, with the already existing **Toolbox** tool window.

![screenshot](https://github.com/zpqrtbnk/SolutionBook/raw/master/Resources/screenshot.png)

It exposes a treeview of solutions, which can be organized and grouped into folders.

It automatically saves its structure (to `%USERPROFILE%\AppData\Roaming\SolutionBook.settings`) when needed, and also updates itself whenever that file changes,
in a way that *should* guarantee safe concurrent usage between multiple instances of Visual Studio.

### INSTALL

The extension is available through the [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=ZpqrtBnk.SolutionBook) and can be installed
in Visual Studio via the **EXTENSIONS** | **Manage Extensions** menu. Search for "SolutionBook" in online extensions.

### DISCLAIMER

This was all quickly put together, and then hacked and hacked again. WPF is hard. I am not especially proud of the code. There *may* be issues. It works on my 
machine and I use it everyday. Feel free to comment, suggest, propose changes, etc.