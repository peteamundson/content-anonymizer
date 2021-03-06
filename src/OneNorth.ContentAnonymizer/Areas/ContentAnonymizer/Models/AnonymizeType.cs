﻿namespace OneNorth.ContentAnonymizer.Areas.ContentAnonymizer.Models
{
    // https://github.com/marak/Faker.js/
    public enum AnonymizeType
    {
        None,
        Custom,

        // Reset
        Clear, // Empty
        Reset, // Standard Value

        // Name
        FirstName,
        LastName,
        Prefix,
        Suffix,

        // Phone
        Phone,
        Fax,
        Mobile,

        // Internet
        Email,
        Url,
        UserName,

        // Date
        Past,
        Future,
        Recent,

        PastDateTime,
        FutureDateTime,
        RecentDateTime,

        Integer,

        // Lorem
        Words,
        Sentence,
        Sentences,
        Paragraph,
        Paragraphs,
        Replace,

        // Address
        Street,
        City,
        State,
        PostalCode,
        Country,
        Latitude,
        Longitude,
        Coordinates,

        // Attachments
        File,
        Image
    }
}