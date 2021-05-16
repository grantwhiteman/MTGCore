﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MTGCore.Dtos.Models;
using MTGCore.Services;
using MTGCore.Services.Interfaces;
using MTGCore.ViewModels;

namespace MTGCore.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private MTGService _mtgService;
        private IMapper _mapper;
        private IManaCostConverter _manaCostConverter;

        public SearchController(MTGService mtgservice, IMapper mapper, IManaCostConverter manaCostConverter)
        {
            _mtgService = mtgservice;
            _mapper = mapper;
            _manaCostConverter = manaCostConverter;
        }

        [EnableCors("MyPolicy")]
        [HttpPost]
        public async Task<IEnumerable<CardDtoWithSymbols>> Post([FromBody] FormViewModel form)
        {
            // TODO(CD): Lazy so re-using the CardDto. Since the CardDto is the model the db table is based off,
            // we should probably create a new specific DTO returning only the data we need on the frontend
            
            var response = await _mtgService.GetCardByName(form.Name);

            var cardList = _mapper.Map<List<CardDtoWithSymbols>>(response);
            
            cardList.ForEach(m => m.manaSymbols = _manaCostConverter.Convert(m.manaCost));

            return cardList;
        }
    }
}
