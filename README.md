# Sitecore Content Anonymizer Module Readme

> Note: This module supports Sitecore 8.1+  For prior versions, please submit an Issue and I will try to add support.  You can also submit a pull request.

## Overview

The Sitecore Content Anonymizer allows anonymization of the field values on items. 
Anonymization is performed per template type on a field by field basis.
The Content Anonymizer allows administrators to anonymize data within Sitecore's content tree. 
A few examples of how data can be anonymized are as follows:

1. Paragraph text replaced with Lorem Ipsom text.
1. First and last names replaced with randomly selected names.
1. Email addresses replaced with new addresses based on the randomly generated names.
1. Dates replaced with randomly generated dates.

There may be several reasons to anonymize data.

1. Data from an existing client implmentation needs to be anonymized so it can be used for a demo.
1. Data from live should not be brought down to development due to confidentiality.  The data can be anonymized.

### Features

Here are the key features of the Data Anonymizer.

1. Anonymize the values of fields on items based on the underlying template.
1. Anonmization is applied to selected items of the selected template type.
1. Only the latest version of the items are anonymized. **All prior versions of an item will be removed.**
1. All language versions are anonymized.  Currently english based content will be used for some anonymization types.
1. Only fields with an anonymization type selected will be anonymized.
1. Only fields with inner values are anonymized.  Fields containing the standard value, default value, fallback value, or inherited value are not.
1. 30+ out of the box field value anonymization formats
1. Basic custom field formats are supported.  This allows combining one or more field values into a string for use on another field.
1. Item renaming based on custom fields is supported.  Items names will be updated to follow the configured Sitecore naming conventions.
1. Global search and replace can be performed on all fields that are not flagged to be anonymized.

> Note: Anonymization is a destructive action.  It is recommended to backup the database(s) before anonymizing.

### Supported Fields

The following field types and anonymization options are supported by the Data Anonymizer.

* Date
    * Past
    * Future
    * Recent
* Integer
    * Random
* Single-Line Text | Multi-Line Text | Rich Text
    * Custom Format
    * Name
        * First
        * Last
        * Suffix
    * Phone
        * Phone
        * Fax
        * Mobile
    * Internet
        * Email
        * URL
        * UserName
    * Date
        * Past
        * Future
        * Recent
    * Lorem
        * Replace
        * Words
        * Sentence
        * Sentences
        * Paragraph
        * Paragraphs
    * Address
        * Street
        * City
        * State
        * PostalCode
        * Country
        * Latitude
        * Longitude
        * Coordinates
    * File
        * Random - Randomly chosen media item.
    * Image
        * Random - Randomly chosen image.

> Note: Other field types are not currently supported.  Relationship based fields such as Droplink and Multi-list, should be anonymized by anonymizing the related item.

### Cautions
1. Only content you chose to be anonymized will be anonymized.  This only includes content in the content tree.  Other content and data will NOT be anonymized.

## Usage

### Select Template
To start, you first need to select a template.
Data is anonymized based on the template selected.
Only items based on the chosen template can be anonymized.
To select a template, start typing the template name in the type-ahead.  
A list of templates should appear that contain the text you typed.
Select the template you desire.

![Select Template](https://raw.github.com/onenorth/content-anonymizer/master/img/select-template.png)

Once the template is chosen, additional options appear.

### Configure Custom Formats
You can optionally configure custom formats.  
Custom formats allow you to combine one or more value into a single string format. 
The resulting string can be used for text based fields or the name of the item.
If you need a custom format, click the **Add** button.
Click Add as many times as you need custom formats.
Each entry needs a **name**.
The name is used when specifying the Item Name or Field format.
The name can be anything you desire.
Each entry also needs at least one **token**.  
The token is tied to a field and represents the chosen field.
The first field, will be assigned "$0" as the token.  
The second "$1", third "$2", and so on.  
The tokens are used in the format string.
A **format** string also needs to be specified.
The format string should contain the tokens and any desired surrounding text.
The **result** shows the interpreted format.
An Example may be as follows for an email address related to person template:
* **Name**: Email
* **Tokens**: $0|FirstName  $1|LastName
* **Format**: $0_$1@domain.com
* **Result**: FirstName_LastName@domain.com

![Configure Custom Formats](https://raw.github.com/onenorth/content-anonymizer/master/img/configure-custom-formats.png)

### Configure Search and Replace
You can define a global search and replace for text based fields, that are not chosen to be anonymized.
If you need to add a search and replace, click the **Add** button.
Click the Add button as many times as you need search and replace.
The content entered in the **Replace** field is replaced with the content entered in the **With** field.
The replacement is case sensitive.

![Configure Search and Replace](https://raw.github.com/onenorth/content-anonymizer/master/img/configure-search-and-replace.png)

### Configure Naming
You can optionally rename the items that are anonymized.
If renaming, check the **Rename Items** checkbox.
Type of the name of the custom format to use and chose the item in the type-ahead.
The type-ahead will search the names provided in the Custom Format section.

![Configure Naming](https://raw.github.com/onenorth/content-anonymizer/master/img/configure-naming.png)

### Configure Fields
Choose what fields you want to anonymize by specifying the type of anonymization.
Fields that do not have a selected anonymization type will not be anonymized.
Relationships remain as-is (Anonymize relationships be anonymizing the target template)

![Configure Fields](https://raw.github.com/onenorth/content-anonymizer/master/img/configure-fields.png)

### Configure Items
Choose which items you want to anonymize.
You can optionally select all with the **all items** checkbox.

![Configure Items](https://raw.github.com/onenorth/content-anonymizer/master/img/configure-items.png)

### Run
To run the anonymization, click the **Anonymize** button.
Note: the anonymize button appears disabled if required fields are not populated.
Please make sure all required fields have been filled out.
You will see a confirmation dialog appear that summarises the selections.
If everythink looks ok, click the **Anonymize** button on the dialog.

## Installation

Install the update packages located here: https://github.com/onenorth/data-anonymizer/tree/master/release

## Configuration

There are no configuration files associated with this module.

#License

The associated code is released under the terms of the [MIT license](http://onenorth.mit-license.org).

The Sitecore Content Anonymizer was inspired by and has used data definitions from:
* https://github.com/marak/Faker.js/ - Copyright (c) 2014-2015 Matthew Bergman & Marak Squires 