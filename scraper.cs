using System.IO.Compression;

namespace LeBonCoinAlert;

internal static class Scraper
{
    public static async Task<string> FetchPageContent(string url)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:128.0) Gecko/20100101 Firefox/128.0");
        request.Headers.Add("Accept", "*/*");
        request.Headers.Add("Accept-Language", "de,en-US;q=0.7,en;q=0.3");
        request.Headers.Add("Accept-Encoding", "gzip, deflate, br, zstd");
        // request.Headers.Add("Referer",
        //     $"https://www.leboncoin.fr/recherche?category=10&text=logement&locations=Rennes__48.10980730273463_-1.6674540604783095_7662_5000&price=min-450&page={pageNumber}");
        request.Headers.Add("x-nextjs-data", "1");
        request.Headers.Add("Connection", "keep-alive");
        request.Headers.Add("Cookie",
            "datadome=Bm8gDC_dU9gob0ZmDPBmFacsFcrKTgDiA6PF5T6KVd8w~0yvagW4rWl5JPmHAtX9_0npvC05khifAiEJQ0xAfWLM~Qiaq8bKxPSGY8RhlZJ4LsxlLR5FXHe5~lVpQXh3; pa_privacy=^%^22exempt^%^22; utag_main=v_id:01914da837db001b2c9ca8e35a1f05050008c00d00970$_sn:2$_ss:0$_st:1723620737606$_pn:5^%^3Bexp-session$ses_id:1723617973670^%^3Bexp-session; didomi_token=eyJ1c2VyX2lkIjoiMTkxNGRhODMtYTIzZS02MjQzLTlhMGMtZDk5ODM2MGQ3ZDM0IiwiY3JlYXRlZCI6IjIwMjQtMDgtMTNUMjE6MzI6MzYuMjU5WiIsInVwZGF0ZWQiOiIyMDI0LTA4LTEzVDIxOjMyOjQyLjY4M1oiLCJ2ZW5kb3JzIjp7ImVuYWJsZWQiOlsiZ29vZ2xlIiwiYzpsYmNmcmFuY2UiLCJjOnJldmxpZnRlci1jUnBNbnA1eCIsImM6cHVycG9zZWxhLTN3NFpmS0tEIiwiYzppbmZlY3Rpb3VzLW1lZGlhIiwiYzp0dXJibyIsImM6YWRpbW8tUGhVVm02RkUiLCJjOmdvb2dsZWFuYS00VFhuSmlnUiIsImM6dW5kZXJ0b25lLVRManFkVHBmIiwiYzptNnB1YmxpY2ktdFhUWUROQWMiLCJjOnJvY2tlcmJveC1mVE04RUo5UCIsImM6YWZmaWxpbmV0IiwiYzpzcG9uZ2VjZWxsLW55eWJBS0gyIiwiYzp0YWlsdGFyZ2UtbkdXVW5heTciLCJjOnRpa3Rvay1yS0FZRGdiSCIsImM6emFub3gtYVlZejZ6VzQiLCJjOnBpbnRlcmVzdCIsImM6aWduaXRpb25vLUxWQU1aZG5qIiwiYzpkaWRvbWkiLCJjOmxiY2ZyYW5jZS1IeTNrWU05RiJdfSwicHVycG9zZXMiOnsiZW5hYmxlZCI6WyJleHBlcmllbmNldXRpbGlzYXRldXIiLCJtZXN1cmVhdWRpZW5jZSIsInBlcnNvbm5hbGlzYXRpb25tYXJrZXRpbmciLCJwcml4IiwiZGV2aWNlX2NoYXJhY3RlcmlzdGljcyIsImNvbXBhcmFpc28tWTNaeTNVRXgiXX0sInZlbmRvcnNfbGkiOnsiZW5hYmxlZCI6WyJnb29nbGUiLCJjOnB1cnBvc2VsYS0zdzRaZktLRCIsImM6dHVyYm8iXX0sInZlcnNpb24iOjIsImFjIjoiRExXQThBRVlBTElCN2dFbGdRREFpU0JLUURFUUhUZ09yQWdZQkJ1Q0tnRWM0Skp3UzFnbXRCUVlDaEVGRm9LNTRXQ2hZTUMxVUZ0NExnd1hHQXVXQmdNRENJR1dvQUFBLkRMV0E4QUVZQUxJQjdnRWxnUURBaVNCS1FERVFIVGdPckFnWUJCdUNLZ0VjNEpKd1MxZ210QlFZQ2hFRkZvSzU0V0NoWU1DMVVGdDRMZ3dYR0F1V0JnTURDSUdXb0FBQSJ9; consent_bucket=full; include_in_experiment=true; _pctx=^%^7Bu^%^7DN4IgrgzgpgThIC4B2YA2qA05owMoBcBDfSREQpAeyRCwgEt8oBJAE0RXSwH18yBbAGYALAMYQAVsPwAfVAC8IADwBuAJgCsfAL5A; ry_ry-l3b0nco_realytics=eyJpZCI6InJ5XzI3REFBMjRDLUU2MjgtNDZGMi05NzQwLUZBODZGQjk1MzE3NSIsImNpZCI6bnVsbCwiZXhwIjoxNzU1MTIwNzU3NDU4LCJjcyI6MX0^%^3D; euconsent-v2=CQDSlQAQDSlQAAHABBENBBFkAP_gAELgAAAAJ9NB_G_dTSFi8X51YPtgcQ1P4VAjogAABgaJAwwBiBLAMIwEhmAAIAHqAAACABAAIDZAAQBlCADAAAAAYIAAAyAMAAAAIRAIJgAAAEAAAmJICABJCwAgAQAQgkgAABUAgAIAABogSFAAAAAAFAAAACAAAAAAAAAAAAAAQAAAAAAAAgAAAAAACAAAAAAEAFAAAAAAAAAAAAAAAAAEAAAAAAEELwATDQqIACwJCQg0DCAAACoIAgAgAAAAAJAwQAABAgAEAYACjAAAAAFAAAAAAAAEBAAAAAAgAQgAAAAIEAAAAAEAAAAEAgEAAAAAAAAABAAAAAEAIAQAIAAgAAAAAIAQAAgAAgAJCgAAAAAAgAAABAAAAQAEAAAAAAAAAAAAAAAAQAAAAAABADFAAYAAgpiMAAwABBTEgABgACCmIAAA.f_wACFwAAAAA; dblockS=42; dblockV=1; adview_clickmeter=search__listing__36__e2d601f3-efee-459e-a3d7-460bb2724ce0; i18n_resources=0eb0,9efa,f2e2,c94c,6fff,1ea4,91bd,3495,c6e6,1296,a063,e5b0,6dd5,4a7c,55b2,edf4,7d70,5079,a7d9,e4e6,32a8,a6f7,fb44,fcbb,7efc,beeb,b7d6,d628,26d3,3ca2,8a15,ad60,9437,4880,16c7,d760,9a2c,cc5f,b98f,a4a5,edd0,d405,c1bb,1920,7f31,efe2,aa75,1888,e956,68c7,bbec,a1d1,7d49,b915,d755,4638,a394,09bb,b9b3,369b,9fcf,c62f,ded9,9b11,29d1,e226,af2f,e46e,3f7e,9359,280f,ef56,978c,3541,8134; __Secure-Install=65539446-f57c-4b66-a25a-357da1fce11f; deviceId=94e062d4-5158-4474-8ed5-975e9b324613; _pcid=^%^7B^%^22browserId^%^22^%^3A^%^22lzsyoeinyrl1jwqv^%^22^%^2C^%^22_t^%^22^%^3A^%^22mfhdn7y1^%^7Clzsypqm1^%^22^%^7D; _dd_s=rum=0&expire=1723619837183; ry_ry-l3b0nco_so_realytics=eyJpZCI6InJ5XzI3REFBMjRDLUU2MjgtNDZGMi05NzQwLUZBODZGQjk1MzE3NSIsImNpZCI6bnVsbCwib3JpZ2luIjp0cnVlLCJyZWYiOm51bGwsImNvbnQiOm51bGwsIm5zIjp0cnVlLCJzYyI6Im9rIiwic3AiOm51bGx9; datadome=eYwGRkd2o42bhuLvP0pNlVJ~IQ2qzsHFIdy0dihpqOS_b9LfQ6Wp9mAb5YunAK9rtGbqYLHfyV4Lk~SN0tvfMy7dmifXFxfFMWxek3XfNRsNf8W3ZLUsDOrIF4t3RY7V");
        request.Headers.Add("Sec-Fetch-Dest", "empty");
        request.Headers.Add("Sec-Fetch-Mode", "cors");
        request.Headers.Add("Sec-Fetch-Site", "same-origin");
        request.Headers.Add("Priority", "u=4");
        request.Headers.Add("Pragma", "no-cache");
        request.Headers.Add("Cache-Control", "no-cache");
        request.Headers.Add("TE", "trailers");
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var contentStream = await response.Content.ReadAsStreamAsync();

        await using (var decompressedStream = new GZipStream(contentStream, CompressionMode.Decompress))
        using (var reader = new StreamReader(decompressedStream))
        {
            return await reader.ReadToEndAsync();
        }
    }
}