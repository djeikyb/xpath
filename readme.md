a basic xpath util with preset namespaces

it would be more convenient if this util would read a namespace prefix table from a config file

it would be most convenient if literally any other tool did

ie xmllint

<hr>

this commit is tuned to smpte namespaces

so i can poke at an asset map and stub out fake files for each reel's picture and sound files like:

```sh
xpath '//am:Path/text()' ASSETMAP.xml | xargs -I{} touch {}
```
