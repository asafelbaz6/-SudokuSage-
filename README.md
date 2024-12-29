# -SudokuSage-

Challenge your mind and sharpen your logic with Sudoku Sage! Featuring 200 carefully crafted Sudoku grids ranging from easy to expert levels, this sleek and interactive 2D experience is designed for puzzle lovers of all skill levels.

Game Features and Design:
I used a backtracking algorithm to generate the Sudoku puzzles, storing the solutions in a matrix. Each puzzle ensures a single valid solution by strategically removing a set number of cells. The removed cells are saved in a dictionary with Point(i, j) as keys and int removedCell as values, enabling an easy-to-use hint system. For enhanced gameplay, a stack-based undo feature allows players to backtrack their steps seamlessly.

Level Management and Data Handling:
Originally, I utilized Scriptable Objects to implement the levels but switched to a JSON-based system for greater flexibility. I created a SudokuDataContainer class to encapsulate the solution board, the playable board (after cell removal), the deleted positions dictionary, and the map level data. This approach ensures scalability and easier level updates.

Transition and Tooling:
Smooth transitions between game states were achieved using DOTween, while Odin Inspector greatly streamlined the development process by simplifying inspector customization and debugging.

Refactoring and Dependency Injection:
As a practice, I refactored the entire project to improve code quality and maintainability. I incorporated Dependency Injection (DI) with Zenject to decouple dependencies and enhance the modularity of the codebase. Key game systems, such as the puzzle generator, hint system, and level loader, are now instantiated and managed through Zenject, promoting flexibility and ease of testing. This shift has also made the project more extensible, laying a strong foundation for future features and improvements.

Development Insights:
Maintaining clean and organized code was pivotal for quick debugging and efficient feature implementation. This refactoring process not only improved the game’s architecture but also deepened my understanding of modern development patterns.

Overall, building Sudoku Sage and refining it further with modern practices was both a fun and rewarding experience!
