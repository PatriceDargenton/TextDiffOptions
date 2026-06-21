# TextDiffOptions
Options interface for TextDiffToHtml (or WinMerge) comparator in C#
---

TextDiffOptions allows you to add useful options for the TextDiffToHtml file comparator, as well as for WinMerge. The idea is to preprocess the text files to be compared, so as to ignore details that overload the comparison via TextDiffToHtml or WinMerge. For example, if you have non-breaking spaces in one file and not in the other, TextDiffToHtml will display a lot of differences that are probably irrelevant to you. The same applies to case sensitivity (uppercase/lowercase), accents, and punctuation. However, there are still limitations: if the sentences are too long, or if there is a line break within a sentence, the detection no longer works. Therefore, there is an option allowing word-for-word comparison: TextDiffToHtml then finds all the differences without taking punctuation into account, only the words themselves, which is useful, for example, for comparing two versions of a text.

## Table of Contents
- [Keywords](#keywords)
- [Features](#features)
- [Explanations](#explanations)
- [Command line arguments](#command-line-arguments)
- [Versions](#versions)
- [Links](#links)

# Keywords
TextDiff, TextDiffToHtml, WinMerge, Text comparison.

# Features
- Case-insensitive comparison (lowercase/uppercase)
- Comparison ignoring accents
- Comparison ignoring quotation marks
- Comparison ignoring punctuation
- Comparison ignoring sentences

# Explanations

Pagination Ratio: This is an option for paginating texts that assume have identical content, even if they are not the same length. The difference in size may be due, for example, to line breaks on every line in one of the two files (in which case, apply the ratio), or one of the texts may have additional content. In this case, either the content is at the end, or we don't know, and in that case, applying a ratio can also be useful.

Word-for-word comparison: If you need to compare two texts, one of which has excessive line breaks, you should attempt a word-for-word comparison (without punctuation or sentences, just consecutive words). If hyphenation has also been applied, TextDiffOptions can automatically correct these broken words based on statistical analysis: if the repeated word is more frequent than the sum of the broken words, then it's a likely hyphenation that can be corrected. In this mode, the file size after processing is also displayed to give an idea of ​​the similarity between the two documents. To activate this option, uncheck the Paragraph option.

# Command line arguments

- Without argument: TextDiffOptions will then display two buttons to add or remove a shortcut in the "Send To" menu of the file explorer.
- 2 arguments: Full file path of the first text file, Full file path of the second text file: Select two text files in the file explorer and send them to TextDiffOptions.

# Versions

See [Changelog.md](Changelog.md)

# Links

See also: [TextDiffToHtml](https://github.com/PatriceDargenton/TextDiffToHtml)