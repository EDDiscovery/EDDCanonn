# EDD-Canonn Plugin |Work in Progress| Readme AI Generated. Incorrect information expected.

An EDDiscovery plugin integrating features from [Canonn EDMC](https://github.com/canonn-science/EDMC-Canonn) and [EDMC-RingSurvey](https://github.com/canonn-science/EDMC-RingSurvey), adapted for EDDiscovery to support scientific data collection and exploration efforts.

## Overview
This plugin brings the functionality of the [Canonn EDMC](https://github.com/canonn-science/EDMC-Canonn) and [EDMC-RingSurvey](https://github.com/canonn-science/EDMC-RingSurvey) plugins to EDDiscovery. It allows commanders to contribute to scientific research efforts by logging system, body, and ring data, supporting exploration and predictive modeling for visibility and data completeness.

## Features

### User interface Implementation
Implementation of the full [Canonn EDMC](https://github.com/canonn-science/EDMC-Canonn) user interface.

### System Target Info
When selecting a target system in ED, the plugin queries [Spansh](https://spansh.co.uk/plotter) to determine the system's status in the database.

**Status Indicators:**
- **Red:** The system is not in [Spansh](https://spansh.co.uk/plotter).
- **Amber:** The system coordinates are in [Spansh](https://spansh.co.uk/plotter), but no celestial bodies are logged.
- **Light Green:** The system and some celestial bodies are logged in [Spansh](https://spansh.co.uk/plotter).
- **Green:** The system and all its celestial bodies are fully logged in [Spansh](https://spansh.co.uk/plotter).

While this feature increases the chances of finding unexplored systems, it relies on [Spansh](https://spansh.co.uk/plotter) contributions and may not always reflect complete data.

### Ring Survey Integration
This plugin incorporates the functionality of the [EDMC-RingSurvey](https://github.com/canonn-science/EDMC-RingSurvey) plugin, allowing users to collect and verify data about planetary rings for visibility predictions.

**How It Works:**
- When you scan a celestial body with rings, the plugin displays:
  - The body name.
  - Details about its rings, including whether they are visible or invisible.
- The plugin attempts to predict visibility automatically.
- If the prediction is incorrect, you can manually adjust the visibility setting by clicking on the ring entry.
- Once corrected, click the **Submit** button to send the data to the central repository for analysis.

### Additional Features

#### Codex and POI Integration
- Connects to EDSM and [Canonn’s](https://github.com/canonn-science/EDMC-Canonn) databases to provide detailed information about:
  - System features (e.g., Earth-like worlds, geological and biological sites).
  - Points of Interest (POIs) such as Thargoid, Guardian, and Galactic Mapping locations.
- Users can store and manage personal POIs using a CSV file (`my_patrol.csv`) located in the plugin's directory.

#### Hyperdiction and NHSS Reporting
- Logs hyperdiction events and NHSS (Non-Human Signal Source) data from the FSS scanner.
- Stores unique instances of threat levels per system for research purposes.

#### Surface and Signal Data
- Captures FSS signal discoveries, surface biology, Guardian sites, and Thargoid-related data.
- Automatically stores data in a central database for visualization and analysis.
