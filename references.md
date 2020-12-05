# References

The mobile app has a tab for reference documents.  Feel free to add new files with product or technical information that others might find helpful.

## How To Add
1. Navigate to [the pages folder](https://github.com/blaxbb/Micro-C-App/tree/master/micro-c-app/micro-c-app/Assets/Pages)
2. Navigate to the folder where you would like to add a new entry
3. Click add file (If you would like to create a new folder, you can add slashes in the filename to create a new folder. *It must have the file extension of .md (ex: CPUSpecs.md)*
4. Create your file using the [Markdown format](https://guides.github.com/features/mastering-markdown/)  You can use the preview tab to ensure proper formatting.  You can also navigate to any other .md file and click "Raw" to see the source Markdown.
5. Use the submit button at the bottom to submit your new entry.

## How to Edit
1. Navigate to [the pages folder](https://github.com/blaxbb/Micro-C-App/tree/master/micro-c-app/micro-c-app/Assets/Pages)
2. Navigate to the file you would like to edit.
3. Click edit and submit a request.

### Specical Urls
There are some special urls you can use to link to other parts of the app.

* `[Ryzen 5 3600 🔍](search=951970)` Will open up the search page and search for `951970` which will fetch the product page for the Ryzen 5 3600 processor.
* `[Office SKUs 📕](reference=/Office/SKUs)` Will navigate to the reference page at the path `/Office/SKUs`.  The path is just the file path starting at [the pages folder](https://github.com/blaxbb/Micro-C-App/tree/master/micro-c-app/micro-c-app/Assets/Pages) ignoring the .md file extension.
* `[Desktop Extended Warranty 🛠](plan=Desktop_Extension)` Will navigate to the reference page for the Plan Type `Desktop_Extension`  You can see the list of Plan Types [here](https://github.com/blaxbb/Micro-C-App/blob/master/micro-c-lib/Models/Reference/PlanReference.cs#L18)

## Guidelines
* **No confidential information should be submitted.**  This is a public repository for a public app.
* **No personal information for anyone should be submitted**  You may sign an entry you have worked on with your name and store at the bottom of the page.
* Cite sources where applicable, particularly if you are citing information from a reviewer, journalist, or other media source.
* Information should not be specific to any one Micro Center location.
* Good ideas for entries are write ups of frequently asked customer questions, such as compatibility issues between different products.
* Images can be embeded using Markdown, but should be avoided when possible.  Usage rights should be looked at, and public domain images are prefered.  Unstable network conditions will also cause images to load slowly, or not at all.  Symbols and icons should be emoji 👍
* Entries should be created with the limited screen size of mobile in mind.  Text will wrap correctly, but tables with many columns may require horizontal scrolling to view properly, which  is not ideal.
* Information should be purely factual.  Opinions on product quality will not be accepted.  Subjective reviews are not okay.  Objective benchmarks with direct citations are welcome.

Acceptable Benchmark Entry Example:
### Q3Test Performance 32bit color (fps - higher is better)
| | 640x480 | 800x600 | 1024x768 |
|-| :-----: | :-----: | :------: |
| Nvidia TNT2 Ultra | 84.2 | 73.9 | 50.4 |
| Nvidia TNT2 | 81.8 | 63.5 | 41.9 |
| 3dfx Voodoo 3 3000 | 88.7 | 68.4 | 46.5 |
| 3dfx Voodoo 3 2000 | 83.0 | 58.5 | 39.9 |
| ATI Rage 128 Pro | 72.4 | 58.0 | 38.8 |

Source: [Anandtech](https://www.anandtech.com/show/389/8)


