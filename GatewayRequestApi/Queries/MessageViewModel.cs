namespace GatewayRequestApi.Queries;

public record RsiMessageView
{
    public string collection_code { get; init; }
    public string shelfmark { get; init; }
    public string volume_number { get; init; }
    public string storage_location_code { get; init; }
    public string author { get; init; }
    public string title { get; init; }
    public DateTime publication_date { get; init; }
    public DateTime periodical_date { get; init; }
    public string article_line1 { get; init; }
    public string article_line2 { get; init; }
    public string catalogue_record_url { get; init; }
    public string further_details_url { get; init; }
    public string dt_required { get; init; }
    public string route { get; init; }
    public string reading_room_staff_area { get; init; }
    public string seat_number { get; init; }
    public string reading_category { get; init; }
    public string reader_name { get; init; }
    public int reader_type { get; init; }
    public string operator_information { get; init; }
    public string item_identity { get; init; }
    public string identifier { get; set; }

    public RsiMessageView()
    {
        collection_code = string.Empty;
        shelfmark = string.Empty;
        volume_number = string.Empty;
        storage_location_code = string.Empty;
        author = string.Empty;
        title = string.Empty;
        publication_date = default;
        periodical_date = default;
        article_line1 = string.Empty;
        article_line2 = string.Empty;
        catalogue_record_url = string.Empty;
        further_details_url = string.Empty;
        dt_required = string.Empty;
        route = string.Empty;
        reading_room_staff_area = string.Empty;
        seat_number = string.Empty;
        reading_category = string.Empty;
        reader_name = string.Empty;
        reader_type = default;
        operator_information = string.Empty;
        item_identity = string.Empty;
        identifier = string.Empty;
    }
}