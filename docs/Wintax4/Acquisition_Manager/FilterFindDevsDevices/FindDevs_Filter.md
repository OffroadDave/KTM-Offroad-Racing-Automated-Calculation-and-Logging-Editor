## Device filtering

The user can manually add a list of prefixes, separated from each other by a comma symbol ( , ) to define an inclusive list of devices that need to be considered when using the download functions. These prefixes are compared against the list of device aliases detected and visualized in the acquisition manager list of devices.

#### Configuration:

To enable the filter function you need to enable the "Filter Device Name" flag. Once it's enabled, you will forcefully need to enter a value. 

If the related text field is empty when the configuration screen is validated an error message will be visualized.

If you want to enter multiple prefixes, ensure that each prefix is separated from the previous one by a comma value.

#### Manual selection mode:

The filter button is enabled. The "Filter Device Name" flag in the acquisition setting has already been enabled.

After the filter has been configured in the configuration screen of the acquisition manager, the button indicated above can be used to quickly enable / disable the function. 

When the filter is enabled you will only see devices with aliases that match the prefixes you entered in the "Filter Device Name" text field. When the filter function is enabled, the device combobox will be surrounded by a red rectangle.

#### ANY Mode:

The "Any" mode button and the filter button are both enabled. The "Filter Device Name" flag in the acquisition setting has already been enabled.

The "Any" mode will consider exclusively devices corresponding to one of the configured prefixes. The order of prefixes will be considered when applying the filter.

Example:

Available devices: Alias10, Logger1

Configured filter: Logger, Alias

The device that is automatically selected when the ANY mode is enabled will therefore be "Logger1" because it corresponds to the first entered prefix.