# Sitecore Computed Search
The module is an index search development accelerator that allows developers to declare computed search fields on an index that can capture and store targeted field values of items within a scope as well as the field values of items based on specific templates that are part of its presentation.

In addition, specific weights can by applied to these computed fields using search boosting so that content matching search terms can be promoted in search results.

Ultimately, developers will be able to implement a content search on their indexes using these computed search fields with Sitecore's ContentSearch API.

This module supports both Lucene and Solr search engines.

You can watch this video that shows what this module does: https://youtu.be/F3p6t43FaZc

### Why would I use it?
If you are looking to implement a lightweight, targeted and flexible search for your Sitecore website, and Coveo, Google Site Search or Google Search Appliance aren't options, this module can be your champion.

### How does it work?
Sitecore provides the framework to build our solutions with an emphasis on granularity and reusability. As a result, the items that represent our website's pages contain renderings that output content from the item, as well as content from items that could be in almost any location within the content tree.

This module gives you the ability to target **ANY and ALL** of the content that makes up an item's presentation, and store it in a computed field within your index so that you can implement a content search using the Sitecore ContentSearch API.

### Why can't we simply suck in and store the HTML that is generated at runtime?
The problem with this approach as that you get all of the page's content, including global elements such as the content in the header and footer. As a result, the extra crud would cause your results to be inaccurate. You also can't apply boosting to targeted content with this approach.

## Complete Flexibility 
The module gives you complete flexibility to add what you want to your search index fields:

* **Scope**
 * If you only want the crawler to focus on a specific area of the tree when performing it's search content indexing.
* **Allow No Presentation**
 * Crawler will include items that are configured that don't contain presentation components. 
* **Templates and Field Name Matching** 
 * Crawler will only include templates and fields on the template that you want to include in your computed search field.
* **Content Sanitizing**
 * Removes all html tags from the final search computed field.

You can also declare as many computed "crawler" fields as you like!

### Custom Pipeline
The module is implemented via a Custom Pipeline so that you can extend its functionality by adding your own processors or replacing the existing ones that are part of this module.

## Configuration
The module is compatible with both Lucene and Solr search indexes and includes sample configurations for both. It works for any of Sitecore's OOTB indexes, as well as your custom indexes.

### Sample Field Configuration
This sample was used in a demonstration using the Launch Sitecore website:

```
<?xml version="1.0"?>
<configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
  <sitecore>
    <contentSearch>
      <indexConfigurations>
        <defaultLuceneIndexConfiguration type="Sitecore.ContentSearch.LuceneProvider.LuceneIndexConfiguration, Sitecore.ContentSearch.LuceneProvider">
          <fields hint="raw:AddComputedIndexField">
            <field fieldName="PageSearchContent" boost="2f" type="Arke.Sitecore.ComputedSearch.ComputedFields.ContentSearch.ComputedSearchContent, Arke.Sitecore.ComputedSearch" scopeItemId="{110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9}" allowNoPresentation="false" templateFieldNames="title|subtitle|body">
              <visualCrawlerFieldMappings>
                <visualCrawlerFieldMap templateId="{B65CCE04-BCCB-4A58-B988-753D523C99A7}" templateFieldNames="title|abstract|body" />
              </visualCrawlerFieldMappings>
            </field>
          </fields>
        </defaultLuceneIndexConfiguration>
      </indexConfigurations>
    </contentSearch>
  </sitecore>
</configuration>
```

### Fields
The following field node attributes are available for configuration:

* **FieldName**
 * The name of the computed field.
* **Boost**
 * The search boost value.
* **Scopeitemid**
 * The Sitecore id root of your crawler's scope i.e., where it should start looking for items to store search content.
* **AllowNoPresentation**
 * A boolean string value that will determine if your crawler will crawl and store content for items that have no presentation components.
* **TemplateFieldNames**
 * Pipe-separated field names of the item whose values you want to store in the computed field.

### VisualCrawlerFieldMappings 
These are the mappings for content items that are part of the presentation details, brought in via rendering datasources.

The following VisualCrawlerFieldMap node attributes are available for configuration:
 
 * **TemplateId**
  * Items part of presentation that contain the template id to be targeted.
 * **TemplateFieldNames**
  * Pipe-separated names of fields whose values will be stored in the computed field.

## Pipeline Description
### Compute Presentation Pipeline

* **EnsureCrawlerMappings**
  * Confirm that mapping confguration and scope is valid
* **EnsureSitePresentation**
 * Confirm that item has presentation components.
 * If it does not contain presentation components, a check is performed to confirm that it doesn't need to contain presentation components before proceeding.
* **GetItemContent**
 * Retrieve targeted field content from the item.
* **GetVisualizationContent**
 * Retrieve the targeted field content from items that are part of presentation.
* **SanitizeSearchContent**
 * Remove html tags and other junk content from the targeted content.

### Pipeline Arguments
* **CurrentItem**
 * Item that is currently being evaluated.
* **ComputedContent**
 * Targeted content.
* **CrawlerFieldMap**
 * Object that contains configuration properties populated from the index config file.

## Debugging
Setting the Arke.Sitecore.ComputedSearch.DebugMode boolean string value to "true", will output useful debugging information to the Sitecore logs such as:

* Field name matches found for items indicating the template type.
* Number of datasources found for items where a configuration match is identified.
* Number of datasources found for the descendants of item where a configuration match is identified.
* Field name matches for found items referenced by a datasource of item where a configuration match is identified.

Log entries will be prefixed with [Computed Search] to make them easy to find.

Sample:
```
5340 22:15:11 INFO  [Computed Search] 1 dataSources found for the descendents of item 'The-Launch-Sitecore-Site' that match template 'Articles Section'
```

## Installation

The Sitecore package Arke.Sitecore.ComputedSearch-1.0.0.zip contains:

* Binary (release build).
* Configuration file containing custom Compute Presentation Pipeline.
* Sample Lucene and Solr config files that are disabled. 
  * These configurations work with the Launch Sitecore demo website: http://www.launchsitecore.net/

Use the Sitecore Installation Wizard to install the package. 

**After installation**:

* Enable either the ComputedLuceneIndexFieldSample for Lucene or ComputedSolrIndexFieldSample for Solr.
* Configure your target "crawler" fields.
* Rebuild your target index using Indexing Manager within Control Panel.

At this point, if you use Luke or Solr Admin, you will be able to see your "computed content" in the field that you have defined.

Next step would be for you to write some LINQ code using Sitecore's ContentSearch API!

## Sitecore ContentSearch API Computed Field Usage Sample Code
This is a **very** basic snippet of code to demonstrate how to consume a computed search field.

### POCO Class

```
public class PageSearchItem : SearchResultItem
{

    [IndexField("PageSearchContent")]
    public string PageSearchContent { get; set; }
    
}
```

### LINQ Search
```
public List<Item> GetSearchPageContent(string searchKeyword)
{
    var index = Sitecore.ContentSearch.ContentSearchManager.GetIndex("custom_index");
    
    using (Sitecore.ContentSearch.IProviderSearchContext context = index.CreateSearchContext())
    {
         var predicate = PredicateBuilder.True<PageSearchItem>();
         predicate = predicate.And(p => p.Name != "__Standard Value");
         predicate = predicate.And(p => p.PageSearchContent.Contains(searchKeyword.ToLowerInvariant()));       
         
         var results = context.GetQueryable<PageSearchItem>().Where(predicate).GetResults();
         return results.Hits.Select(hit => hit.Document.GetItem()).ToList();
    }

    return null;            
}
```
