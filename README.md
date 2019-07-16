# dangeasy-umbraco-visual-studio

# Setup Instructions
1. Fork/copy this repository, then build in Visual Studio
2. Run application and go through standard Umbraco setup WITHOUT a starter kit
3. Go to Settings section -> usych, and click Import
4. Go to Content section, create Root and Home nodes.




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


## Umbraco
### Core DocumentTypes
- Site Container (houses Site and Repository Container)
- Site (requires selection of Home node)
	- Configuration (lives in Site Root node)
- Repository Container (for housing of partial content/elements)	


### Page DocumentTypes
- Home (lives under Site Root)
- Base Page (implements Page Settings Mixin)
	- Page Settings Mixin (includes SEO & scripts)



### Content Node Structure
The node structure 

Site Container
 - Website (contains hostname)
   - Home
   - Eror404
 - Data Repositories	
	
Data repositories are a common way to store partial content/elements which need to be referenced across your website. 
For example:
- Taxonomies 
   - Categories 
   - News Tags 
   - Global Tags 	 
- Locations 
- Authors 
- External Links 



## Umbraco Packages (TBD)
- USync
- Meganav
- SiteLock?
