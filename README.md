# dangeasy-umbraco-visual-studio

# What is this?
A base Visual Studio solution for Umbraco 8.1+.  

The solution is a SOLID foundation of common patterns, folder structures, and website features which are widely accepted as "best practice" in the Umbraco community. 

Note: This is not meant to be a starter kit, nor is it meant to have a load of features which are toggled. 


# Base Functionality
## Visual Studio Structure
- Application.Web - Umbraco
- Application.Core - Builders, Services
- Application.Models - Models Builder, other application models

## Web.config
### Redirects
- Remove "/""
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
- Site Root (requires selection of Home node)
- Configuration (lives in Site Root)
- Home (lives under Site Root)
- Seo Mixin
- Base Page (implements Seo Mixin)


## Umbraco Packages (TBD)
- USync
- Meganav
- SiteLock?
