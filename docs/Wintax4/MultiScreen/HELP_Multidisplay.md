### Multi-Display / Layout viewer

**Layout Viewers configuration**

A layout viewer allows the visualization of a layout outside the main windows of the program.

These windows can be configured to be visualized in any of the connected displays, without limitations.

When creating one or more external layout viewers it is possible to visualize multiple layouts at the same time.

All Layout viewers will show the same data that has been loaded in WinTAX, will share the cursor position, zoom levels and other general settings for WinTAX, for example the background colors or the windows layout.

When WinTAX is closed and reopened, the external layout viewers will be configured to maintain the same position that was configured before WinTAX was closed.

Below it is possible to see a sample configuration visualized on 2 different screens.

****

**Layout viewer creation:**

It is possible to generate a new layout viewer in 2 different ways:

(The same menu is also available in the contextual menu of the layout toolbar.).

The "**New Layout Viewer"**** command creates a viewer in the same window where the main WinTAX window is visualized. It is then possible to transfer the window to the desired display.**

The “New Layout Viewer on display” command asks the user to select the destination display where the layout viewer is desired. The window that will appear will visualize the detected displays. It is then possible to click on one of the displays to immediately create a viewer in it.

**Layout viewer configuration:**

****

The layout viewer is composed of the following elements:

Reduced Menu: contains only functions to operate inside the same window.

Layouts Toolbar: This toolbar visualizes the same layouts that are also visible in the main window. All modifications made to this toolbar are immediately replicated on the toolbar of all the other viewers and on the main windows.

Graph Toolbar: It's the toolbar that contains the commands of the selected windows inside the layout viewer. 

**Multi-Monitor configuration environment**

The new "Layout viewers manager" environment allows to easily operate on a multi-monitor system, by saving and restoring the positions and configuration schemes of the main windows and layout viewers.

This feature is particularly useful when the notebook or the PC are frequently connected and disconnected to different monitor (for example a notebook that frequently changes location and is used with a "track" and "office" configuration, where the number and sizes of monitors might be completely different. On the left side of the window, it is possible to see all saved configurations. Also, indicated in red, the "Current configuration" is always synchronized with the actual configuration that can be seen on the screen.

Using the menu on the lower side of the screen it is possible to:

  * Create a copy of the selected configuration (for a following modification)
  * Remove a configuration
  * Sort all configurations
  * Enable a configuration
  * Export all selected configurations (all exported configuration can then be imported on other computers using the function "Import").



Enabling a configuration allows, when the environment is closed to create exactly the configuration of the main windows and of the external layouts that are visible in the preview area of the window. 

Selecting a layout viewer in the preview window, it is possible to "Hide" temporarily a viewer that is not needed. It is possible to restore it in another moment without losing its configuration.

All changes made to the active configuration will be visible when the window is closed.

**Activating a pre-saved configuration**

Using the button “Activate” in the lowest side of the window or the relative function in the context menu, a popup will inform the user that when the configuration window is closed using the OK button the selected configuration will be automatically visualized on the connected monitors.

In this image it is possible to see that the "Pitlane configuration" has been activated: when confirmed with the OK button, all windows will be visualized as shown in the preview window.

If the layout configuration that is being activated has one or more layout viewers that cannot be visualized will be automatically configured as "hidden" and a warning message will be visualized.

It is then possible to open the "Layout viewers manager" and check the "Current configuration" to see which viewers have been changed to hidden.

**Connection and disconnection of displays:**

WinTAX is capable of detecting connections and disconnection of displays while it's running and to adapt automatically to the available configuration.

While being used the following situations could happen:

**One monitor is disconnected from the PC:**

In this situation WinTAX detects which layout viewers are visualized on the disconnected monitor and sets them automatically to hidden while maintaining the configuration. It is also possible to verify the configuration by checking the Layout Viewer Manager:

**One monitor is connected to the PC:**

In this situation WinTAX detects if the actual active configuration contains layout viewers that have been not visualized because the related monitor was missing. If this is the case, they will be automatically restored on the connected monitor.

This feature allows to never lose any configuration if a screen is connected or disconnected while WinTAX is being executed.