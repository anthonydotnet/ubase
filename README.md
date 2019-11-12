# ubase - What is it?
A base Visual Studio solution for Umbraco 8.1+.  
The solution is a SOLID foundation of common patterns, folder structures, and website features which are widely accepted as "best practice" in the Umbraco community. USync definitions in this solution provide a foundation set of document types and datatypes. 

Note: This is not meant to be a "starter kit" per se, nor is it meant to have a load of features which are toggled. 

# Setup Instructions
1. Fork/copy this repository, then build in Visual Studio
2. Run application and go through standard Umbraco setup WITHOUT a starter kit
3. Go to Settings section -> usych, and click Import
4. Go to Content section, create Root and Home nodes.


# Base Functionality
## Visual Studio Structure
- Application.Web - Umbraco
- Application.Core - Builders, Services
- Application.Models - Models Builder, other application models

## Web.config
### Redirects
- Remove "/""
- Redirect to https (enabled by default)
- Internal rewrite to /ErrorPages/500.html

### Optimisations
- Gzip static files

## Common Pages
- 500.html (static)
- Robots.txt (content managed)
- Sitemap.xml (generated)


## Umbraco
### Core DocumentTypes
- Site Container (houses Site and Repository Container)
- Site (requires selection of Home node)
	- Configuration (lives in Site node)
- Repository Container (for housing of partial content/elements)	


### Page DocumentTypes
- Home (lives under Site Root)
- Base Page (implements Page Settings Mixin)
	- Page Settings Mixin (includes SEO & scripts)



### Content Node Structure
A base node structure is provided which allows flexibility, multi-websites and separation of partial data elements.

- Site Container
  - Website (put hostname here)
    - Home
    - Eror404
  - Data Repositories	
	
Note: Data repositories are a common way to store partial content/elements which need to be referenced across your website. 
For example:
- Taxonomies 
   - Categories 
   - News Tags 
   - Global Tags 	 
- Locations 
- Authors 
- External Links 


## Umbraco Packages 
- USync
- Meganav (TBD)
- SiteLock (TBD)
