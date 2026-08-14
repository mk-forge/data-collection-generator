# Data Collection & Query Generator

Web application for generating data collections, query collections and statistical functions.

## Overview

This project was developed as my bachelor thesis at VŠB-TUO. It builds on an existing tool for generating data collections and queries, originally written by a previous student. I fixed performance and stability issues, added support for generating query collections from decimal data collections, and replaced the original command-line interface with a proper Blazor Server GUI.

## Performance and Stability

The original tool was RAM‑only, so it worked well for small datasets but failed on larger ones. I switched to batch disk I/O, added memory mapped files, parallel sorting, and caching. This makes it a bit faster for small datasets and, more importantly, allows it to handle large collections without crashing. It trades a little raw speed for a lot of stability and scalability.

## Tech stack

- Frontend: Blazor Server, Bootstrap, HTML, CSS, JavaScript
- Backend: C#, .NET
- Core library: C++, wrapped with SWIG

## Features

- Authentication against the university LDAP server
- Import existing data collections
- Generate data collections (uniform, normal, lognormal, diagonal, Sierpiński, bit)
- Generate query collections (point, partial match, narrow range, range, cartesian range)
- Generate and visualize histograms
- Sort and shuffle data collections
- Export to files (`.ctf` for data collections, `.qtf` for query collections, `.txt` for histograms, `.sql` for SQL INSERT/SELECT queries)
- Progress bar for long-running operations
- Pagination for large collections and histograms, so the browser doesn't crash
- Dark/light theme toggle

## Screenshots

![Login page](./Screenshots/login.png)
![Data collections page](./Screenshots/collections.png)
![Histogram](./Screenshots/histogram.png)
![Query collections page](./Screenshots/queries.png)

## Installation

1. Download the latest `DataCollectionGenerator.exe` from [Releases](https://github.com/mk-forge/data-collection-generator/releases).
2. Run the executable.
3. The application opens at `http://localhost:5000`.

## Features

- Authentication with university LDAP server
- Import data collections
- Generate data collections (uniform, normal, lognormal, diagonal, Sierpiński, bit)
- Generate query collections (point, partial match, narrow range, range, cartesian range)
- Generate & visualize histograms
- Sort & shuffle data collections
- Export to files (`.ctf` for data collections, `.qtf` for query collections, `.txt` for histograms, `.sql` for SQL INSERT queries and SQL SELECT queries)
- Real-time progress bar for long operations
- Pagination for large collections and histograms (to prevent browser overload)
- Toggle for dark/light theme