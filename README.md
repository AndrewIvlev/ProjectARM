# ProjectARM
### Investigation of algorithms for local-optimal motion planning of robot manipulators

The solution has two projects. Application in ArmManipulatorApp and tests for it in ArmManipulatorApp_Tests.
The ArmManipulatorApp project is built on the architecture of the MVVM pattern.
There we have 'View' by MainWindow.xaml, 'ViewModel' is ApplicationViewModel.cs and the 'Model' is divided into graphic model (ArmManipulator/Graphics) and mathematical model (ArmManipulator/MathModel).
ApplicationViewModel there is 