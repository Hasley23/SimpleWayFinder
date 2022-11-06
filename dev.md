# SimpleWayFinder | Project structure | Visual Studio 2019

**SimpleWayFinder** app consists of three main parts: *MainWindow*, *ColorCell*(with XAML markup) and *SWFCore* (program logic class with **A\* algo**).

1.  *MainWindow:*
    This class contains user interface of the app (main form with buttons) and main logic:
![Editor](/pic/editor.png)
    
2.  *ColorCell:*
     This class contains custom control element – *cell* that have enum with different states and colors (EMPTY, OBSTACLE, START, END, PATH):
![CellStates](/pic/CellStates.png)

3. *SWFCore*:
     This class contains a *position structure* definition, a search graph class definition *(GridGraph class)* and an *SearchEngine* class with heuristic function and *A\** algorithm realization.




