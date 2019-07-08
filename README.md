# dangeasy-umbraco-visual-studio

# What is this?
A base Visual Studio solution for Umbraco 8.1+.  

The solution is a SOLID foundation of common patterns, folder structures, and website features which are widely accepted as "best practice" in the Umbraco community. 

Note: This is not meant to be a starter kit, nor is it meant to have a load of features which are toggled. 


# Base Functionality
## Web.config
### Redirects
- Redirect to "/""
- Redirect to https (enabled by default)
- Redirect /umbraco to /404 (for content delivery instance)
- Internal rewrite to /ErrorPages/500.html

### Optimisations
- Gzip static files

## Common Pages
- 500.html (static)
- Robots.txt (content managed)
- Sitemap.xml (generated)


## Umbraco (TBD)
### DocumentTypes
- Site Root (allows multiple instances)
- Home (lives under Site Root)
- Configuration (lives in Site Root)
- Seo Mixin
- Base Page (implements Seo Mixin)


## Umbraco Packages
- USync
- Meganav
- SiteLock?
