#include <mono-wasi/driver.h>
#include <assert.h>
#include <string.h>

// MonoMethod* method_HandleIncomingRequest;
MonoMethod* method_Start;

__attribute__((export_name("start")))
void start(char* workingDirectory, int port)
{
    if (!method_Start)
    {
        method_Start = lookup_dotnet_method(
            "RJDev.MemoryMappedCache.dll",
            "RJDev.MemoryMappedCache",
            "Interop",
            "Start",
            -1
        );
        assert(method_Start);
    }

    MonoObject* exception;
    void* method_params[] = {mono_wasm_string_from_js(workingDirectory), &port};
    mono_wasm_invoke_method(method_Start, NULL, method_params, &exception);
    assert(!exception);

    free(workingDirectory);


    // bool result = *(bool *)mono_object_unbox(result);
    // return result;
}

// void run (MonoObject* dotnet_http_server, MonoString* workingDirectory, int port) {
//     char* workingDIrectory_utf8 = mono_wasm_string_get_utf8(workingDirectory);
//     start (dotnet_http_server, workingDIrectory_utf8, strlen(workingDIrectory_utf8), port);
// }

// __attribute__((import_name("response_send_chunk")))
// void response_send_chunk (int request_id, void* buffer, int buffer_len);
//
// __attribute__((import_name("response_add_header")))
// void response_add_header (int request_id, char* name, int name_len, char* value, int value_len);
//
// __attribute__((import_name("response_complete")))
// void response_complete (int request_id, int status_code);


// MonoClass* mono_get_byte_class(void);
// MonoDomain* mono_get_root_domain(void);
//
// MonoArray* mono_wasm_typed_array_new(void* arr, int length) {
//     MonoClass* typeClass = mono_get_byte_class();
//     MonoArray* buffer = mono_array_new(mono_get_root_domain(), typeClass, length);
//     memcpy(mono_array_addr_with_size(buffer, 1, 0), arr, length);
//     return buffer;
// }

// __attribute__((export_name("start")))
// void start(MonoObject* dotnet_polling_server, int request_id, char* method, char* url, char* headers_combined, void* body_ptr, int body_len) {
//     if (!method_HandleIncomingRequest) {
//         method_HandleIncomingRequest = lookup_dotnet_method("RJDev.MemoryMappedCache.dll", "RJDev.MemoryMappedCache", "Interop", "Start", -1);
//         assert(method_HandleIncomingRequest);
//     }
//
//     MonoArray* body_dotnet_array = body_ptr
//         ? mono_wasm_typed_array_new(body_ptr, body_len)
//         : NULL;
//
//     void* method_params[] = { &request_id, mono_wasm_string_from_js(method), mono_wasm_string_from_js(url), mono_wasm_string_from_js(headers_combined), body_dotnet_array };
//     MonoObject *exception;
//     mono_wasm_invoke_method (method_HandleIncomingRequest, dotnet_polling_server, method_params, &exception);
//     assert (!exception);
//     free(method);
//     free(url);
//     free(headers_combined);
// }

// void response_add_header_mono(int request_id, MonoString* name, MonoString* value) {
//     char* name_utf8 = mono_wasm_string_get_utf8(name);
//     char* value_utf8 = mono_wasm_string_get_utf8(value);
//     response_add_header(request_id, name_utf8, strlen(name_utf8), value_utf8, strlen(value_utf8));
// }

// void fake_settimeout(int timeout) {
//     // Skipping
// }

void attach_internal_calls()
{
    // mono_add_internal_call ("RJDev.MemoryMappedCache.Interop::RunHttpServer", run);
    // mono_add_internal_call ("RJDev.MemoryMappedCache.Interop::ResponseSendChunk", response_send_chunk);
    // mono_add_internal_call ("RJDev.MemoryMappedCache.Interop::ResponseAddHeader", response_add_header_mono);
    // mono_add_internal_call ("RJDev.MemoryMappedCache.Interop::ResponseComplete", response_complete);

    // mono_add_internal_call ("System.Threading.TimerQueue::SetTimeout", fake_settimeout);
}
